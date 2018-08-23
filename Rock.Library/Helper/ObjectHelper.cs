using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Rock.Library.Helper
{
    public class ObjectHelper
    {
        /// <summary>
        /// 获取对象属性
        /// </summary>
        private static PropertyInfo[] GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// 实体值自动映射赋值
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源数据实体</param>
        /// <param name="target">待赋值实体</param>
        /// <param name="isAllMapping">是否完全映射（默认是）</param>
        /// <param name="excepts">排除的属性名称</param>       
        public static void MappingData<S, T>(S source, T target, bool isAllMapping = true, params string[] excepts)
        {
            Type sType = source.GetType();
            Type tType = target.GetType();

            //属性映射
            var sProperties = sType.GetProperties();
            foreach (var tProperty in tType.GetProperties())
            {
                if (excepts != null && excepts.Any(p => p.ToLower() == tProperty.Name.ToLower())) continue;
                if (!isAllMapping && tProperty.GetValue(target) != null) continue;

                var sProperty = sProperties.FirstOrDefault(p => p.Name.ToLower() == tProperty.Name.ToLower());
                if (sProperty == null) continue;
                //object[] attributes = tProperty.GetCustomAttributes(typeof(NotAutoMappingAttribute), true);
                //if (attributes != null && attributes.Length > 0) continue;
                if (tProperty.PropertyType == sProperty.PropertyType)
                    tProperty.SetValue(target, sProperty.GetValue(source));
            }

            //字段映射
            var sFields = sType.GetFields();
            foreach (var tField in tType.GetFields())
            {
                if (excepts != null && excepts.Any(p => p.ToLower() == tField.Name.ToLower())) continue;
                if (!isAllMapping && tField.GetValue(target) != null) continue;

                var sField = sFields.FirstOrDefault(p => p.Name.ToLower() == tField.Name.ToLower());
                if (sField == null) continue;
                //object[] attributes = tField.GetCustomAttributes(typeof(NotAutoMappingAttribute), true);
                //if (attributes != null && attributes.Length > 0) continue;
                if (tField.FieldType == sField.FieldType)
                    tField.SetValue(target, sField.GetValue(source));
            }
        }

        /// <summary>
        /// 表转实体对象
        /// </summary>
        public static List<T> DataTableToEntities<T>(DataTable dataTable) where T : class, new()
        {
            var bag = new ConcurrentBag<T>();
            var properties = typeof(T).GetProperties().Where(p => p.GetMethod.IsVirtual == false).ToArray();
            var map = GetMapping<T>(dataTable, properties);
            try
            {
                Parallel.ForEach(dataTable.AsEnumerable(), row =>
                {
                    var entity = new T();
                    foreach (var key in map.Keys)
                    {
                        var property = properties[map[key]];
                        property.SetValue(entity, row[key] is DBNull ? null : row[key]);
                    }
                    bag.Add(entity);
                });
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
            return bag.ToList();
        }

        private static Dictionary<int, int> GetMapping<T>(DataTable dataTable, PropertyInfo[] properties)
        {
            var map = new Dictionary<int, int>();
            var proLen = properties.Length;
            var colLen = dataTable.Columns.Count;
            for (int j = 0; j < colLen; j++)
            {
                for (int x = 0; x < proLen; x++)
                {
                    if (dataTable.Columns[j].ColumnName.ToLower() == properties[x].Name.ToLower())
                    {
                        map.Add(j, x);
                    }
                }
            }
            return map;
        }

        #region 反射行数

        #region 通过反射执行方法
        /// <summary>
        /// 通过反射执行方法
        /// </summary>
        /// <param name="dllName">DLL名称</param>
        /// <param name="className">类名称</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public static object InvokeMethod(string dllName, string className, string methodName, object[] parameters)
        {
            Assembly asmb = Assembly.LoadFrom(dllName);
            Type t = asmb.GetType(className);
            object dObj = Activator.CreateInstance(t);
            MethodInfo method = t.GetMethod(methodName);
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;

            object returnValue = method.Invoke(dObj, flag, Type.DefaultBinder, parameters, null);
            return returnValue;
        }
        #endregion

        


        #endregion

    }
}
