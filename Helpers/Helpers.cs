using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using System;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace SkillMastery.Helpers
{
  public static class SkillMasteryHelper
  {
    private static Func<object, Hero> _cachedGetter;

    internal static Hero GetContextHero(PerkVM vm)
    {
      try
      {
        // Get delegate from PerkVM
        var field = AccessTools.Field(typeof(PerkVM), "_getIsPerkSelected");
        var del = field.GetValue(vm) as Func<PerkObject, bool>;

        if (del == null || del.Target == null)
          return null;

        var target = del.Target; // SkillVM instance

        // Cache getter for this type
        if (_cachedGetter == null)
          _cachedGetter = BuildGetter(target.GetType());

        if (_cachedGetter == null)
          return null;

        return _cachedGetter(target);
      }
      catch
      {
        return null;
      }
    }

    private static Func<object, Hero> BuildGetter(Type skillVmType)
    {
      //
      // FIRST: Look for direct "Hero" property on SkillVM
      //
      var directHeroProp = skillVmType.GetProperty("Hero",
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

      if (directHeroProp != null && directHeroProp.PropertyType == typeof(Hero))
      {
        return delegate (object obj)
        {
          try { return directHeroProp.GetValue(obj) as Hero; }
          catch { return null; }
        };
      }

      //
      // SECOND: Search SkillVM members for one containing a "Hero" property
      //
      var members = skillVmType.GetMembers(
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

      for (int i = 0; i < members.Length; i++)
      {
        MemberInfo m = members[i];
        Type mType = null;

        if (m is FieldInfo)
          mType = ((FieldInfo)m).FieldType;
        else if (m is PropertyInfo)
        {
          var p = (PropertyInfo)m;
          if (p.GetIndexParameters().Length == 0) // no indexers
            mType = p.PropertyType;
        }

        if (mType == null)
          continue;

        // look for a "Hero" property on this member's type
        var heroProp = mType.GetProperty("Hero",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (heroProp != null && heroProp.PropertyType == typeof(Hero))
        {
          // FOUND the developer-like object → return a getter
          return BuildSubGetter(m, heroProp);
        }
      }

      // Nothing found
      //InformationManager.DisplayMessage(new InformationMessage("SkillMastery: Unable to locate Hero reference on SkillVM"));
      return null;
    }

    private static Func<object, Hero> BuildSubGetter(MemberInfo member, PropertyInfo heroProp)
    {
      return delegate (object obj)
      {
        try
        {
          object subObj = null;

          if (member is FieldInfo)
            subObj = ((FieldInfo)member).GetValue(obj);
          else if (member is PropertyInfo)
            subObj = ((PropertyInfo)member).GetValue(obj, null);

          if (subObj == null)
            return null;

          return heroProp.GetValue(subObj, null) as Hero;
        }
        catch
        {
          return null;
        }
      };
    }
  }
}