/*
 * Source: http://blogs.msdn.com/b/arvindsh
 * 
 * This Sample Code is provided for the purpose of illustration only and is not intended to be used in a production environment.  THIS SAMPLE CODE AND ANY RELATED INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.  We grant You a nonexclusive, royalty-free right to use and modify the Sample Code and to reproduce and distribute the object code form of the Sample Code, provided that You agree: (i) to not use Our name, logo, or trademarks to market Your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on Your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend Us and Our suppliers from and against any claims or lawsuits, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code.

This posting is provided "AS IS" with no warranties, and confers no rights. Use of included script samples are subject to the terms specified at http://www.microsoft.com/info/cpyright.htm.

 */

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Reflection;

namespace Database.Core.IO
{
    public static class XmlVisualizer
    {
        static StringBuilder result = new StringBuilder();

        public static string Present(TSqlFragment tree) {
            ScriptDomWalk(tree, "root");
            return result.ToString();
        }

        private static void ScriptDomWalk(object fragment, string memberName)
        {
            if (fragment.GetType().BaseType.Name != "Enum")
            {
                result.AppendLine("<" + fragment.GetType().Name + " memberName = '" + memberName + "'>");
            }
            else
            {
                result.AppendLine("<" + fragment.GetType().Name + "." + fragment.ToString() + "/>");
                return;
            }

            Type t = fragment.GetType();

            PropertyInfo[] pibase;
            if (null == t.BaseType)
            {
                pibase = null;
            }
            else
            {
                pibase = t.BaseType.GetProperties();
            }

            foreach (PropertyInfo pi in t.GetProperties())
            {
                if (pi.GetIndexParameters().Length != 0)
                {
                    continue;
                }

                if (pi.PropertyType.BaseType != null)
                {
                    if (pi.PropertyType.BaseType.Name == "ValueType")
                    {
                        result.Append("<" + pi.Name + ">" + pi.GetValue(fragment, null) + "</" + pi.Name + ">");
                        continue;
                    }
                }

                if (pi.PropertyType.Name.Contains(@"IList`1"))
                {
                    if ("ScriptTokenStream" != pi.Name)
                    {
                        var listMembers = pi.GetValue(fragment, null) as IEnumerable<object>;

                        foreach (object listItem in listMembers)
                        {
                            ScriptDomWalk(listItem, pi.Name);
                        }
                    }
                }
                else
                {
                    object childObj = pi.GetValue(fragment, null);
                 
                    if (childObj != null)
                    {
                        if (childObj.GetType() == typeof(string))
                        {
                            result.Append(pi.GetValue(fragment, null));
                        }
                        else
                        {
                            ScriptDomWalk(childObj, pi.Name);
                        }
                    }
                }
            }

            result.AppendLine("</" + fragment.GetType().Name + ">");
        }

    }
}
