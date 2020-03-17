using System;

namespace cfgConfig.Core.Validation
{
    internal static class Validator
    {
        public static void ValidateAll(params object[] objs)
        {
            foreach (object obj in objs)
                Validate(obj);
        }

        public static void ValidateAll(params string[] strs)
        {
            foreach (string str in strs)
                Validate(str);
        }

        public static void Validate(object variable)
        {
            if (variable == null)
                throw new ArgumentNullException("Variable cannot be null.");
        }

        public static void Validate(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentNullException("String cannot be null or contain only whitespaces.");
        }
    }
}
