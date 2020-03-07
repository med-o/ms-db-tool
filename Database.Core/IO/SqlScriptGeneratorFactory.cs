using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Database.Core.IO
{
    public class SqlScriptGeneratorFactory : ISqlScriptGeneratorFactory
    {
        public SqlScriptGenerator Get(SqlVersion version)
        {
            // TODO : upgrade ScriptDom package version to get access to newer syntax?

            SqlScriptGeneratorOptions options = new SqlScriptGeneratorOptions
            {
                SqlVersion = version,
                KeywordCasing = KeywordCasing.Uppercase
            };

            switch (version)
            {
                case SqlVersion.Sql80:
                    return new Sql80ScriptGenerator(options);
                case SqlVersion.Sql90:
                    return new Sql90ScriptGenerator(options);
                case SqlVersion.Sql100:
                    return new Sql100ScriptGenerator(options);
                case SqlVersion.Sql110:
                    return new Sql110ScriptGenerator(options);
                case SqlVersion.Sql120:
                default:
                    return new Sql120ScriptGenerator(options);
            }            
        }
    }
}
