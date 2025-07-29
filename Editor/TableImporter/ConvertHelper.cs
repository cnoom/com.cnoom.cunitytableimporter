using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace cnoom.Editor.TableImporter
{
    /// <summary>
    /// ConvertHelper 使用 TypeConverter 统一解析各种类型，包括自定义泛型列表
    /// </summary>
    public static class ConvertHelper
    {
        // 类型名到 Type 的映射
        private static readonly Dictionary<string, Type> typeMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { "int", typeof(int) },
            { "float", typeof(float) },
            { "string", typeof(string) },
            { "bool", typeof(bool) },
            { "List<int>", typeof(List<int>) },
            { "List<float>", typeof(List<float>) },
            { "List<string>", typeof(List<string>) },
            { "List<bool>", typeof(List<bool>) },
        };

        static ConvertHelper()
        {
            // 注册泛型列表转换器
            TypeDescriptor.AddAttributes(typeof(List<int>), new TypeConverterAttribute(typeof(ListTypeConverter<int>)));
            TypeDescriptor.AddAttributes(typeof(List<float>), new TypeConverterAttribute(typeof(ListTypeConverter<float>)));
            TypeDescriptor.AddAttributes(typeof(List<string>), new TypeConverterAttribute(typeof(ListTypeConverter<string>)));
            TypeDescriptor.AddAttributes(typeof(List<bool>), new TypeConverterAttribute(typeof(ListTypeConverter<bool>)));
        }

        /// <summary>
        /// 根据类型名解析字符串值
        /// </summary>
        public static object ParseValue(string typeName, string value)
        {
            if (!typeMap.TryGetValue(typeName, out Type type))
            {
                Debug.LogWarning($"未知的类型：{typeName}");
                return null;
            }

            var converter = TypeDescriptor.GetConverter(type);
            try
            {
                return converter.ConvertFromString(value);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"类型解析失败: {typeName} 值: {value}, 异常: {ex.Message}");
                return null;
            }
        }
    }
    
    public class ListTypeConverter<T> : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string str)
            {
                if (string.IsNullOrEmpty(str))
                    return new List<T>();

                var elements = str.Split(';');
                var list = new List<T>(elements.Length);
                var elementConverter = TypeDescriptor.GetConverter(typeof(T));
                foreach (var item in elements)
                {
                    list.Add((T)elementConverter.ConvertFromString(item));
                }
                return list;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}