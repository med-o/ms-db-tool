using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Context;
using Database.Core.Generator;
using Database.Core.IO;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Validation;

namespace Database.App.WinForms.UI
{
    public partial class ApplicationForm : Form
    {
        private readonly IValidationEngine _validationEngine;
        private readonly IDatabaseSchemaGenerator _generator;
        private readonly ILocalFileSchemaGenerator _localGenerator;
        private readonly IParser _parser;
        private readonly IDatabaseContextProvider _contextProvider;
        private readonly ILogger _logger;
        private readonly IFormatter _formatter;
        private readonly IRichTextBoxStreamWriterFactory _textStreamWriterFactory;

        private SchemaDefinition _schema;

        public ApplicationForm(
            IValidationEngine validationEngine, 
            IDatabaseSchemaGenerator generator,
            ILocalFileSchemaGenerator localGenerator,
            IParser parser,
            IDatabaseContextProvider contextProvider,
            ILogger logger,
            IFormatter formatter,
            IRichTextBoxStreamWriterFactory textStreamWriterFactory
        )
        {
            _validationEngine = validationEngine;
            _generator = generator;
            _localGenerator = localGenerator;
            _parser = parser;
            _contextProvider = contextProvider;
            _logger = logger;
            _formatter = formatter;
            _textStreamWriterFactory = textStreamWriterFactory;

            InitializeComponent();

            RedirectConsoleOutputToTextBox();

            CenterToScreen();

            SetInitialScript();

            DisplayCheckboxesForValidationRules();

            CreateSchema();
        }

        private void RedirectConsoleOutputToTextBox()
        {
            var writer = _textStreamWriterFactory.Create(outputTextBox);
            Console.SetOut(writer);
        }

        private void CreateSchema()
        {
            _schema = _generator.GenerateSchema();
        }

        private void SetInitialScript()
        {
            sqlTextBox.Text  = @"
USE someDatabase
GO

CREATE TABLE someTable (
  column1 INT NOT NULL,
  column2 VARCHAR
)
GO

CREATE TABLE dbo.anotherTable (
  col1 BIGINT NOT NULL,
  col2 DECIMAL
)
GO

SELECT *, column1, col2
FROM dbo.someTable T1
JOIN anotherTable T2
  ON T1.column1 = T2.col1
WHERE column2 LIKE ""%Hello World%""";
        }

        private void DisplayCheckboxesForValidationRules()
        {
            var grouppedValidationRules = _validationEngine
                .GetValidationRules()
                .GroupBy(x => x.Type, (key, group) => group.First())
                .GroupBy(x => x.Settings.Level);

            foreach (var group in grouppedValidationRules)
            {
                var groupBox = new GroupBox()
                {
                    Text = string.Format("{0}:", group.Key),
                    Font = new Font("Consolas", 10, FontStyle.Bold),
                    MinimumSize = new Size(480, 50),
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                };

                var flowPanel = new FlowLayoutPanel()
                {
                    FlowDirection = FlowDirection.TopDown,
                    Location = new Point(10, 20),
                    MinimumSize = new Size(460, 50),
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                };
                
                foreach (var rule in group)
                {
                    var ruleCheckBox = new CheckBox() { 
                        Text = rule.Settings.Label,
                        Checked = rule.Settings.Enabled,
                        Font = new Font("Consolas", 10, FontStyle.Regular),
                        AutoSize = true,
                    };

                    ruleCheckBox.CheckedChanged += 
                        (Object sender, EventArgs e) => 
                            rule.Settings.Enabled = rule.Settings.Enabled ? false : true;

                    flowPanel.Controls.Add(ruleCheckBox);
                }

                groupBox.Controls.Add(flowPanel);

                settingsPanel.FlowDirection = FlowDirection.TopDown;
                settingsPanel.WrapContents = false;
                settingsPanel.Controls.Add(groupBox);
            }
        }

        private void validateButton_Click(object sender, EventArgs e)
        {
            outputTextBox.Clear();

            _logger.Log(LogLevel.Information, "Parsing ... start");
            var parserOutput = _parser.ParseString(sqlTextBox.Text);
            _logger.Log(LogLevel.Information, "Parsing .. end");
            _logger.Log(string.Empty);

            var fileName = "local file"; // TODO : use the name of the file that you have loaded

            if (parserOutput.ParsingErrors != null && parserOutput.ParsingErrors.Count > 0)
            {
                _logger.LogParsingErrors(parserOutput, fileName);
            }
            else
            {
                // TODO : this should by configurable
                // NOTE : formatting removes comments we rely on to get database context !!!
                if (false)
                {
                    _logger.Log(LogLevel.Information, "Script formatting ... start");
                    var formattedSql = _formatter.FormatSql(parserOutput.TsqlScript, SqlVersion.Sql120);
                    parserOutput = _parser.ParseString(formattedSql);
                    sqlTextBox.Text = formattedSql;
                    _logger.Log(LogLevel.Information, "Script formatting ... end");
                    _logger.Log(string.Empty);
                }

                // TODO : grabbing first context, for SSMS use the database dropdown
                var databaseContext = _contextProvider
                    .Get(sqlTextBox.Text, parserOutput.TsqlScript)
                    .First();

                var file = new SchemaFile()
                {
                    Context = databaseContext,
                    Path = fileName,
                    Schema = _schema,
                    Settings = null, // TODO : seems like I don't need settings here for now
                    TsqlScript = parserOutput.TsqlScript,
                };
                _localGenerator.AddLocalSchema(file);

                _logger.Log(LogLevel.Information, "Validate SQL ... start");
                _logger.Log(string.Empty);
                var validationResults = _validationEngine.ValidateFile(file);
                _logger.Log(string.Empty);
                _logger.Log(LogLevel.Information, validationResults.Any(vr => vr.Value.Any()) 
                    ? "SQL is not valid" 
                    : "SQL is valid"
                );
                _logger.Log(string.Empty);
                _logger.LogValidationErrors(validationResults, file);
                _logger.Log(LogLevel.Information, "Validate SQL .. end");
            }
        }

        private void clearOutputTextAreaButton_Click(object sender, EventArgs e)
        {
            outputTextBox.Clear();
        }

        private void resetScriptButton_Click(object sender, EventArgs e)
        {
            SetInitialScript();

            using (var selectFileDialog = new OpenFileDialog())
            {
                if (selectFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var sr = new StreamReader(selectFileDialog.OpenFile()))
                    {
                        var fileContent = sr.ReadToEnd();
                        sqlTextBox.Text = fileContent;
                    }
                }
            }
        }

        private void buttonViewDOM_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "XML file|*.xml";
            saveDialog.Title = "Save script DOM";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveDialog.FileName, false))
                {
                    var parserOutput = _parser.ParseString(sqlTextBox.Text);
                    var DOM = XmlVisualizer.Present(parserOutput.TsqlScript);
                    sw.Write(DOM);
                }
            }
        }

        private void refreshSchemaButton_Click(object sender, EventArgs e)
        {
            CreateSchema();
        }
    }
}
