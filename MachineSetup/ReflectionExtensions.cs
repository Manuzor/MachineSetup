using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MachineSetup
{
    public static class ReflectionExtensions
    {
        public static object GetValue(this MemberInfo member, object obj)
        {
            switch(member)
            {
                case PropertyInfo prop: return prop.GetValue(obj);
                case FieldInfo field: return field.GetValue(obj);

                default: throw new ArgumentException($"Expected a MemberInfo of type {MemberTypes.Field} or {MemberTypes.Property}, got {member.MemberType}", nameof(member));
            }
        }

        public static void SetValue(this MemberInfo member, object obj, object value)
        {
            switch(member)
            {
                case PropertyInfo prop: prop.SetValue(obj, value); break;
                case FieldInfo field: field.SetValue(obj, value); break;

                default: throw new ArgumentException($"Expected a MemberInfo of type {MemberTypes.Field} or {MemberTypes.Property}, got {member.MemberType}", nameof(member));
            }
        }

        public static Type GetFieldOrPropertyType(this MemberInfo member)
        {
            switch(member)
            {
                case PropertyInfo prop: return prop.PropertyType;
                case FieldInfo field: return field.FieldType;

                default: throw new ArgumentException($"Expected a MemberInfo of type {MemberTypes.Field} or {MemberTypes.Property}, got {member.MemberType}", nameof(member));
            }
        }

        public static IEnumerable<MemberInfo> GetMembers(this Type type, MemberTypes types)
        {
            return from MemberInfo member in type.GetMembers()
                   where types.HasFlag(member.MemberType)
                   select member;
        }
    }
}
