﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Waddle.Constants;

namespace Waddle.Supports
{
    public static class EnumSupport
    {
        public static string GetStringValue(this SecurityNamespaceConstants value)
        {
            var enumType = typeof(SecurityNamespaceConstants);
            var memberInfos = enumType.GetMember(value.ToString());
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
            var valueAttributes =
                  enumValueMemberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            var description = ((DescriptionAttribute)valueAttributes[0]).Description;
            return description;
        }
    }
}
