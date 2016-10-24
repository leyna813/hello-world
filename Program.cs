using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;
using System.Reflection.Emit;

namespace ReflectionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Program prg = new Program();
            //get reflectioninfo
            Console.WriteLine("-----output reflectionTest information.------");
            GetReflectionInfo();
            ////Please use the refection to create instance of DemoClass
            ////Please use the refection to set the DemoClass instance Field as string "Hello world".
            Console.WriteLine("-----set the DemoClass instance Field as string \"Hello world\".------");
            AssemblyCreateInstance();
            ////Please use the refection to Invoke the MethodB
            Console.WriteLine("-----Invoke the MethodB.------");
            MethodInfoInvoke();

            ////Define an Attribute and try to get it out from Democlass
            //GetSelfAttribute();
           //change here
            Console.ReadKey();
        }

        private static void GetReflectionInfo()
        {
            Type testType = typeof(DemoClass);
            Assembly assembly = testType.Assembly;            
            Console.WriteLine("Assembly：" + assembly.FullName);
            Type[] typeList = assembly.GetTypes();   // get type
            foreach (Type type in typeList)
            {
                Console.WriteLine("-------------" + type.Namespace + " " + type.Name + "----------");

                ////Please use the reflection to output all DemoClass property.                                                
                PropertyInfo[] properties = type.GetProperties();
                Console.WriteLine("public properties：");
                int num1 = 1;
                foreach (PropertyInfo pro in properties)
                {
                    Console.Write((num1++).ToString() + ". " + pro.PropertyType.Name + " " + pro.Name + "{");
                    if (pro.CanRead) Console.Write("get;");
                    if (pro.CanWrite) Console.Write("set;");
                    Console.WriteLine("}");
                }

                ////Please use the reflection to output all DemoClass method name.
                MethodInfo[] methods = type.GetMethods();
                Console.WriteLine("public methods");
                int num2 = 1;
                foreach (MethodInfo method in methods)
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    ParameterInfo reparam = method.ReturnParameter;
                    Console.Write((num2++).ToString() + ". " + reparam.ParameterType.Name + " " + method.Name + "(");
                    int index = 0;
                    foreach (ParameterInfo para in parameters)
                    {
                        if (index++ < parameters.Length - 1)
                            Console.Write(para.ParameterType.Name + " " + para.Name + ",");
                        else
                            Console.Write(para.ParameterType.Name + " " + para.Name);
                    }
                    Console.WriteLine(")");
                }
            }
        }
        private static void GetSelfAttribute()
        {
            Type objType = typeof(SelfAttribute);            
            Console.WriteLine("下面列出应用于 {0} 的SelfAttribute属性：", objType);
            object[] objAttrs = objType.GetCustomAttributes(typeof(SelfAttribute), true);
            foreach (SelfAttribute selfAttr in objAttrs)
            {
                Console.WriteLine("   {0}", selfAttr);
                Console.WriteLine("      类型：{0}", selfAttr.RecordType);
                Console.WriteLine("      作者：{0}", selfAttr.Author);
                Console.WriteLine("      日期：{0}", selfAttr.Date.ToShortDateString());
                if (!String.IsNullOrEmpty(selfAttr.Memo))
                {
                    Console.WriteLine("      备注：{0}", selfAttr.Memo);
                }

            }
            PropertyInfo[] properties = objType.GetProperties();
            int index1=1;
            foreach (PropertyInfo pro in properties)
            {
                Console.Write((index1++).ToString() + ". " + pro.PropertyType.Name + " " + pro.Name + "{");
                if (pro.CanRead) Console.Write("get;");
                if (pro.CanWrite) Console.Write("set;");
                Console.WriteLine("}");
            }

            MethodInfo[] methods = objType.GetMethods();
            Console.WriteLine("public methods");
            int index2 = 1;
            foreach (MethodInfo method in methods)
            {
                ParameterInfo[] parameters = method.GetParameters();
                ParameterInfo reparam = method.ReturnParameter;
                Console.Write((index2++).ToString() + ". " + reparam.ParameterType.Name + " " + method.Name + "(");
                int index = 0;
                foreach (ParameterInfo para in parameters)
                {
                    if (index++ < parameters.Length - 1)
                        Console.Write(para.ParameterType.Name + " " + para.Name + ",");
                    else
                        Console.Write(para.ParameterType.Name + " " + para.Name);
                }
                Console.WriteLine(")");
            }

        }

        ////Please use the refection to create instance of DemoClass
        // 使用Assembly的CreateInstance方法来取得对象的实例
        private static void AssemblyCreateInstance()
        {
            string assemblyName = "ReflectionTest";
            string className = assemblyName + ".DemoClass";
            // 创建无参数实例
            DemoClass demoIns = (DemoClass)Assembly.Load(assemblyName).CreateInstance(className);
            Console.WriteLine("创建无参数实例：" + demoIns);

            ////Please use the refection to set the DemoClass instance Field as string "Hello world".
            string fieldValue = "Hello world";
            PropertyInfo[] fields = demoIns.GetType().GetProperties();
           
            foreach (PropertyInfo pro in fields)
            {
                if (pro != null && !pro.CanWrite)
                {
                    continue;
                }
                //取出实体属性的数据类型
                Type targetType = pro.PropertyType;
                Type convertType = targetType;

                //判断是否是可空类型，如 int?等
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    //可空类型
                    NullableConverter nullableConverter = new NullableConverter(targetType);
                    convertType = nullableConverter.UnderlyingType;
                }
                //对fieldvalue进行数据类型转换

                if (!string.IsNullOrEmpty(convertType.FullName))
                {
                    object value = Convert.ChangeType(fieldValue, convertType);

                    //赋值
                    if (value != null)
                    {
                        pro.SetValue(demoIns, value, null);
                        string value_New = (string)pro.GetValue(demoIns, null);

                        Console.WriteLine(value_New);
                    }
                }


            }




        }

        ////Please use the refection to set the DemoClass instance Field as string "Hello world".




        ////Please use the refection to Invoke the MethodB
        // MethodeInfo对象的 Invoke方法来动态调用方法
        private static void MethodInfoInvoke()
        {
            string assemblyName = "ReflectionTest";
            string className = assemblyName + ".DemoClass";
            string methodName = String.Empty;
            string result = String.Empty;
            Assembly assem = Assembly.Load(assemblyName);
            Object obj = assem.CreateInstance(className);
            Type type = assem.GetType(className);
            // 动态调用无参数的方法                                     
            methodName = "MethodB";
            MethodInfo methodInfo = type.GetMethod(methodName);
            result = (string)methodInfo.Invoke(obj, null);

            //Console.WriteLine(methodName + "方法的返回值：" + result);
            // 动态调用有参数的方法
            /*
            methodName = "MethodA";
            Object[] methodParams = new Object[1];
            methodParams[0] = "test";
            MethodInfo method = type.GetMethod(methodName);
            result = (string)method.Invoke(obj, methodParams);
            Console.WriteLine(methodName + "方法的返回值：" + result);
            */

        }

       


    }

    ///[***]
    public class SelfAttribute: Attribute
    {
        private string recordType;      // 记录类型：更新/创建   
        private string author;          // 作者   
        private DateTime date;          // 日期   
        private string memo;         // 备注   

        // 构造函数，构造函数的参数在特性中也称为“位置参数”。  
        public SelfAttribute(string recordType, string author, string date)
        {
            this.recordType = recordType;
            this.author = author;
            this.date = Convert.ToDateTime(date);
        }

        // 对于位置参数，通常只提供get访问器   
        public string RecordType { get { return recordType; } }
        public string Author { get { return author; } }
        public DateTime Date { get { return date; } }

        // 构建一个属性，在特性中也叫“命名参数”   
        public string Memo
        {
            get { return memo; }
            set { memo = value; }
        }
    }

    [Self("更新", "Matthew", "2008-1-20", Memo = "修改 ToString()方法")]   
    public class DemoClass
    {
        public string Field1 { get; set; }

        public string Field2 { get; set; }

        public string Field3 { get; set; }

        public void MethodA(string text)
        {
            Console.WriteLine(text);
        }

        public void MethodB()
        {
            Console.Write("MethodB Invoked.");
        }
    }


}
