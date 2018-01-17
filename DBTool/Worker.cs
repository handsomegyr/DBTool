#region Using directives

using System.IO;
using System.Text;
using System;
using System.Windows.Forms;
using System.Xml.Serialization;

#endregion

namespace DBTool
{
    public class Worker
    {

        public static bool merge(TableSettings tableSettings, System.ComponentModel.BackgroundWorker backgroundWorker)
        {
            // Start the search for primes and wait.
            UTF8Encoding utf8 = new UTF8Encoding(false);
            //App\Common\Models\XXX
            if (!Directory.Exists(Path.Combine(Application.StartupPath, tableSettings.TableName, "App", "Common", "Models", tableSettings.FolderName)))
            {
                //\Mysql
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, tableSettings.TableName, "App", "Common", "Models", tableSettings.FolderName, "Mysql"));
                //\Mongodb
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, tableSettings.TableName, "App", "Common", "Models", tableSettings.FolderName, "Mongodb"));
            }
            //App\XXX
            if (!Directory.Exists(Path.Combine(Application.StartupPath, tableSettings.TableName, "App", tableSettings.FolderName)))
            {
                //\Models
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, tableSettings.TableName, "App", tableSettings.FolderName, "Models"));
                //\Services
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, tableSettings.TableName, "App", tableSettings.FolderName, "Services"));
                //\Views\Helpers
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, tableSettings.TableName, "App", tableSettings.FolderName, "Views", "Helpers"));
            }
            //App\Backend\Submodules\XXX
            if (!Directory.Exists(Path.Combine(Application.StartupPath, tableSettings.TableName, "App", "Backend", "Submodules", tableSettings.FolderName)))
            {
                //\Models
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, tableSettings.TableName, "App", "Backend", "Submodules", tableSettings.FolderName, "Models"));
            }

            var targetPath1 = Path.Combine(Application.StartupPath, tableSettings.TableName, "schema.php");
            var targetPath2 = Path.Combine(Application.StartupPath, tableSettings.TableName, "mysql.sql");
            var targetPath3 = Path.Combine(Application.StartupPath, tableSettings.TableName + "_settings.xml");
            //App\Common\Models\XXX\Mysql\XXX2.php
            var targetPath4 = Path.Combine(Application.StartupPath, tableSettings.TableName, "App", "Common", "Models", tableSettings.FolderName, "Mysql", tableSettings.ClassName+".php");
            //App\Common\Models\XXX\XXX2.php
            var targetPath5 = Path.Combine(Application.StartupPath, tableSettings.TableName, "App", "Common", "Models", tableSettings.FolderName, tableSettings.ClassName + ".php");

            //App\XXX\Models\XXX2.php
            var targetPath6 = Path.Combine(Application.StartupPath, tableSettings.TableName, "App",  tableSettings.FolderName, "Models", tableSettings.ClassName + ".php");

            //App\Backend\Submodules\XXX\Models\XXX2.php
            var targetPath7 = Path.Combine(Application.StartupPath, tableSettings.TableName, "App", "Backend", "Submodules", tableSettings.FolderName, "Models", tableSettings.ClassName + ".php");

            var writer1 = new StreamWriter(targetPath1, false, utf8);
            var writer2 = new StreamWriter(targetPath2, false, utf8);
            var writer3 = new StreamWriter(targetPath4, false, utf8);
            var writer4 = new StreamWriter(targetPath5, false, utf8);
            var writer5 = new StreamWriter(targetPath6, false, utf8);
            var writer6 = new StreamWriter(targetPath7, false, utf8);

            try
            {
                // 写配置
                XmlSerializer xs = new XmlSerializer(typeof(TableSettings));
                StreamWriter sw = new StreamWriter(targetPath3);
                xs.Serialize(sw, tableSettings);
                sw.Close();

                int i = 0;
                StringBuilder sbSchema = new StringBuilder("");
                StringBuilder sbMysql = new StringBuilder("");
                StringBuilder sbReorganize = new StringBuilder("");
                foreach (var f in tableSettings.FieldSettingsList)
                {
                    i++;
                    if (backgroundWorker.CancellationPending)
                    {
                        // Return without doing any more work.
                        throw new Exception("用户取消了操作");
                    }

                    if (backgroundWorker.WorkerReportsProgress)
                    {
                        backgroundWorker.ReportProgress(i);
                    }

                    if (!f.IsSelected) continue;
                    f.Status = "处理中";

                    try
                    {
                        var schema = getSchema(f);
                        sbSchema.AppendLine(schema);
                        
                        var mysql = getMysql(f);
                        sbMysql.Append(mysql);

                        var reorganize = getReorganize(f);
                        sbReorganize.Append(reorganize);
                        

                        f.Status = "处理完成";
                    }
                    catch (Exception e1)
                    {
                        f.Status = "处理失败";
                        throw e1;
                    }
                    finally
                    {

                    }
                }

                writer1.Write(sbSchema.ToString());

                // 写mysql
                StringBuilder sb = new StringBuilder("");
                sb.AppendLine(@"DROP TABLE IF EXISTS `" + tableSettings.TableName + "`;");
                sb.AppendLine(@"CREATE TABLE `" + tableSettings.TableName + "` (");
                sb.AppendLine(@"  `_id` char(24) NOT NULL DEFAULT '' COMMENT 'ID',");
                sb.Append(sbMysql.ToString());
                sb.AppendLine(@"  `__CREATE_TIME__` datetime NOT NULL COMMENT '创建时间',");
                sb.AppendLine(@"  `__MODIFY_TIME__` datetime NOT NULL COMMENT '修改时间',");
                sb.AppendLine(@"  `__REMOVED__` tinyint(1) unsigned NOT NULL DEFAULT '0' COMMENT '是否删除',");
                sb.AppendLine(@"  PRIMARY KEY(`_id`)");
                sb.AppendLine(@") ENGINE = InnoDB DEFAULT CHARSET = utf8 CHECKSUM = 1 DELAY_KEY_WRITE = 1 ROW_FORMAT = DYNAMIC COMMENT = '" + tableSettings.TableComment + "';");
                writer2.Write(sb.ToString());
                
                // 写class1
                StringBuilder sbClass1 = new StringBuilder("");
                sbClass1.AppendLine(@"<?php");
                sbClass1.AppendLine(string.Format(@"namespace App\Common\Models\{0}\Mysql;", (tableSettings.FolderName)));
                sbClass1.AppendLine(@"use App\Common\Models\Base\Mysql\Base;");
                sbClass1.AppendLine(@"class "+ (tableSettings.ClassName) + " extends Base");
                sbClass1.AppendLine(@"{");
                sbClass1.AppendLine(@"    /**");
                sbClass1.AppendLine(@"     * "+ tableSettings.TableComment+ "管理");
                sbClass1.AppendLine(@"     * This model is mapped to the table "+ tableSettings .TableName+ "");
                sbClass1.AppendLine(@"     */");
                sbClass1.AppendLine(@"    public function getSource()");
                sbClass1.AppendLine(@"    {");
                sbClass1.AppendLine(@"        return '"+ tableSettings.TableName + "';");
                sbClass1.AppendLine(@"    }");
                sbClass1.AppendLine(@"    public function reorganize(array $data)");
                sbClass1.AppendLine(@"    {");
                sbClass1.AppendLine(@"        $data = parent::reorganize($data);");
                sbClass1.Append(sbReorganize.ToString());
                sbClass1.AppendLine(@"        return $data;");
                sbClass1.AppendLine(@"    }");
                sbClass1.Append(@"}");
                writer3.Write(sbClass1.ToString());

                StringBuilder sbClass2 = new StringBuilder("");
                sbClass2.AppendLine(@"<?php");
                sbClass2.AppendLine(string.Format(@"namespace App\Common\Models\{0};", (tableSettings.FolderName)));
                sbClass2.AppendLine(@"use App\Common\Models\Base\Base; ");
                sbClass2.AppendLine(@"class " + (tableSettings.ClassName) + " extends Base");
                sbClass2.AppendLine(@"{");
                sbClass2.AppendLine(@"    function __construct()");
                sbClass2.AppendLine(@"    {");
                sbClass2.AppendLine(string.Format(@"        $this->setModel(new \App\Common\Models\{0}\Mysql\{1}());", (tableSettings.FolderName), (tableSettings.ClassName)));
                sbClass2.AppendLine(@"    }");
                sbClass2.Append(@"}");
                writer4.Write(sbClass2.ToString());


                StringBuilder sbClass3 = new StringBuilder("");
                sbClass3.AppendLine(@"<?php");
                sbClass3.AppendLine(string.Format(@"namespace App\{0}\Models;", (tableSettings.FolderName)));
                sbClass3.AppendLine(string.Format(@"class {0} extends \App\Common\Models\{1}\{2}", (tableSettings.ClassName), (tableSettings.FolderName), (tableSettings.ClassName)));
                sbClass3.AppendLine(@"{");
                sbClass3.AppendLine(@"    /**");
                sbClass3.AppendLine(@"     * 默认排序");
                sbClass3.AppendLine(@"     */");
                sbClass3.AppendLine(@"    public function getDefaultSort()");
                sbClass3.AppendLine(@"    {");
                sbClass3.AppendLine(@"        $sort = array(");
                sbClass3.AppendLine(@"            '_id' => -1");
                sbClass3.AppendLine(@"        );");
                sbClass3.AppendLine(@"        return $sort;");
                sbClass3.AppendLine(@"    }");
                sbClass3.AppendLine(@"    /**");
                sbClass3.AppendLine(@"     * 默认查询条件");
                sbClass3.AppendLine(@"     */");
                sbClass3.AppendLine(@"    public function getQuery()");
                sbClass3.AppendLine(@"    {");
                sbClass3.AppendLine(@"        $query = array();");
                sbClass3.AppendLine(@"        return $query;");
                sbClass3.AppendLine(@"    }");
                sbClass3.Append(@"}");
                writer5.Write(sbClass3.ToString());
                
                StringBuilder sbClass4 = new StringBuilder("");
                sbClass4.AppendLine(@"<?php");
                sbClass4.AppendLine(string.Format(@"namespace App\Backend\Submodules\{0}\Models;", (tableSettings.FolderName)));
                sbClass4.AppendLine(string.Format(@"class {0} extends \App\Common\Models\{1}\{2}", (tableSettings.ClassName), (tableSettings.FolderName), (tableSettings.ClassName)));
                sbClass4.AppendLine(@"{");
                sbClass4.AppendLine(@"    use\App\Backend\Models\Base;");
                sbClass4.Append(@"}");
                writer6.Write(sbClass4.ToString());

                return true;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                throw e;
                //return false;
            }
            finally
            {
                writer1.Flush();
                writer1.Close();

                writer2.Flush();
                writer2.Close();

                writer3.Flush();
                writer3.Close();

                writer4.Flush();
                writer4.Close();
                
                writer5.Flush();
                writer5.Close();

                writer6.Flush();
                writer6.Close();
            }
        }

        protected static string getSchema(FieldSettings f)
        {
            var dataType = f.DataDbSettings.SelectDataType.TypeValue;
            if (f.DataDbSettings.SelectDataType.TypeValue == "varchar" || f.DataDbSettings.SelectDataType.TypeValue == "char" || f.DataDbSettings.SelectDataType.TypeValue == "text")
            {
                dataType = "string";
            }

            if (f.DataDbSettings.SelectDataType.TypeValue == "datetime")
            {
                f.DataDbSettings.DefaultValue = "getCurrentTime()";
                f.DataDbSettings.DataLength = 19;
            }

            if (f.DataDbSettings.SelectDataType.TypeValue == "boolean")
            {
                if (string.IsNullOrEmpty(f.DataDbSettings.DefaultValue)){
                    f.DataDbSettings.DefaultValue = "false";
                }
                
                f.DataDbSettings.DataLength = 1;
            }

            if (f.DataDbSettings.SelectDataType.TypeValue == "integer")
            {
                if (string.IsNullOrEmpty(f.DataDbSettings.DefaultValue))
                {
                    f.DataDbSettings.DefaultValue = "0";
                }
                f.DataDbSettings.DataLength = 11;
            }

            if (f.DataDbSettings.SelectDataType.TypeValue == "file")
            {
                f.DataDbSettings.DataLength = 300;
            }

            if (f.DataDbSettings.SelectDataType.TypeValue == "json" || f.DataDbSettings.SelectDataType.TypeValue == "array" || f.DataDbSettings.SelectDataType.TypeValue == "text")
            {
                f.DataDbSettings.DataLength = 1024;

            }
            StringBuilder sb = new StringBuilder("");

            sb.AppendLine(@"$schemas['" + f.FieldName + "'] = array(");
            sb.AppendLine(@"    'name' => '" + f.FieldComment + "',");
            // 数据配置
            sb.AppendLine(@"    'data' => array(");
            sb.AppendLine(@"        'type' => '" + dataType + "',");
            if (f.DataDbSettings.SelectDataType.TypeValue == "datetime") { 
            sb.AppendLine(@"        'defaultValue' => " + f.DataDbSettings.DefaultValue + ",");
            }
            else
            {
            sb.AppendLine(@"        'defaultValue' => '" + f.DataDbSettings.DefaultValue + "',");
            }
            if (f.DataDbSettings.SelectDataType.TypeValue == "file")
            {
            sb.AppendLine(@"        'file' => array(");
            sb.AppendLine(@"            'path' => $this->modelXXX->getUploadPath()");
            sb.AppendLine(@"        ),");
            }
            sb.AppendLine(@"        'length' => " + f.DataDbSettings.DataLength.ToString() + "");
            sb.AppendLine(@"    ),");
            sb.AppendLine(@"    'validation' => array(");
            sb.AppendLine(@"        'required' => " + f.DataFormSettings.IsRequire.ToString().ToLower() + "");
            sb.AppendLine(@"    ),");

            // 表单配置
            sb.AppendLine(@"    'form' => array(");            
            if (f.DataFormSettings.SelectInputType.TypeValue != "partial")
            {
            if (f.DataFormSettings.SelectInputType.TypeValue == "image")
            {
            sb.AppendLine(@"        'input_type' => 'file',");
            }
            else
            {
            sb.AppendLine(@"        'input_type' => '" + f.DataFormSettings.SelectInputType.TypeValue + "',");
            }                    
            }
            else
            {
            sb.AppendLine(@"        'partial' => 'partials/empty',");
            }
            if (f.DataFormSettings.SelectInputType.TypeValue == "radio")
            {
            sb.AppendLine(@"        'items' => $this->trueOrFalseDatas,");
            }
            if (f.DataFormSettings.SelectInputType.TypeValue == "select" || f.DataFormSettings.SelectInputType.TypeValue == "select2")
            {
            sb.AppendLine(@"        'cascade' => '',");
            sb.AppendLine(@"        'items' => function() {");
            sb.AppendLine(@"            return $this->modelXXXX->getAll();");
            sb.AppendLine(@"        },");
            sb.AppendLine(@"        'select' => array(");
            sb.AppendLine(@"            'multiple' => " + f.DataFormSettings.SelectInputType.IsMultiple.ToString().ToLower() + ",");

            if (f.DataFormSettings.SelectInputType.TypeValue == "select2")
            {
            sb.AppendLine(@"            'is_remote_load' => true,");
            sb.AppendLine(@"            'apiUrl' => 'xx/xx/xx/xx'");
            }
            sb.AppendLine(@"        ),");
            }
            sb.AppendLine(@"        'is_show' => " + f.DataFormSettings.IsShow.ToString().ToLower() + "");
            sb.AppendLine(@"    ),");

            // 列表配置
            sb.AppendLine(@"    'list' => array(");            
            if (f.DataDbSettings.SelectDataType.TypeValue == "boolean")
            {
            sb.AppendLine(@"        'list_type' => '1', ");
            }
            if (f.DataFormSettings.SelectInputType.TypeValue == "image")
            {
            sb.AppendLine(@"        'render' => 'img', ");
            }
            if (!string.IsNullOrEmpty(f.DataListSettings.Ajax))
            {
            sb.AppendLine(@"        'ajax' => '"+ f.DataListSettings.Ajax + "', ");
            }
            if (f.DataFormSettings.SelectInputType.TypeValue == "select" || f.DataFormSettings.SelectInputType.TypeValue == "select2")
            {
            sb.AppendLine(@"        'list_data_name' => '" + f.FieldName + "_name',");
            }
            sb.AppendLine(@"        'is_show' => " + f.DataListSettings.IsShow.ToString().ToLower() + "");
            sb.AppendLine(@"    ),");

            // 检索配置
            sb.AppendLine(@"    'search' => array(");            
            sb.AppendLine(@"        'input_type' => '" + f.DataSearchSettings.SelectInputType.TypeValue + "',");
            sb.AppendLine(@"        'condition_type' => '" + f.DataSearchSettings.SelectConditionType.TypeValue + "',");
            sb.AppendLine(@"        'defaultValues' => array(),");
            if (f.DataSearchSettings.SelectInputType.TypeValue == "select" || f.DataSearchSettings.SelectInputType.TypeValue == "select2")
            {
            sb.AppendLine(@"        'cascade' => '',");
            sb.AppendLine(@"        'items' => function() {");
            sb.AppendLine(@"            return $this->modelXXXX->getAll();");
            sb.AppendLine(@"        },");
            }
            sb.AppendLine(@"        'is_show' => " + f.DataSearchSettings.IsShow.ToString().ToLower() + "");
            sb.AppendLine(@"    ),");

            // 导出配置
            sb.AppendLine(@"    'export' => array(");            
            sb.AppendLine(@"        'is_show' => " + f.DataExportSettings.IsShow.ToString().ToLower() + "");
            sb.AppendLine(@"    )");
            sb.AppendLine(@"); ");

            return sb.ToString();
        }
        
        protected static string getMysql(FieldSettings f)
        {
            var dataType = "";
            var defaultString = "";
            var notnull = f.DataDbSettings.IsNotNull ? "NOT NULL" : "";

            if (f.DataDbSettings.SelectDataType.TypeValue == "char")
            {
                dataType = "char(" + f.DataDbSettings.DataLength + ")";
                defaultString = "DEFAULT ''";
            }

            if (f.DataDbSettings.SelectDataType.TypeValue == "varchar")
            {
                dataType = "varchar(" + f.DataDbSettings.DataLength + ")";
                defaultString = "DEFAULT ''";
            }

            if (f.DataDbSettings.SelectDataType.TypeValue == "datetime")
            {
                dataType = "datetime";
                defaultString = "";
            }

            if (f.DataDbSettings.SelectDataType.TypeValue == "boolean")
            {
                dataType = "tinyint(1) unsigned";
                defaultString = "DEFAULT '0'";
            }

            if (f.DataDbSettings.SelectDataType.TypeValue == "integer")
            {
                dataType = "int("+ f.DataDbSettings.DataLength + ") unsigned";
                defaultString = "DEFAULT '0'";
            }

            if (f.DataDbSettings.SelectDataType.TypeValue == "file")
            {
                dataType = "varchar(300)";
                defaultString = "DEFAULT ''";
            }

            if (f.DataDbSettings.SelectDataType.TypeValue == "json" || f.DataDbSettings.SelectDataType.TypeValue == "array" || f.DataDbSettings.SelectDataType.TypeValue == "text")
            {
                dataType = "text";
                defaultString = "";
            }

            StringBuilder sb = new StringBuilder("");            
            sb.AppendLine(@"  `"+ f.FieldName + "` "+ dataType + " "+ notnull + " "+ defaultString + " COMMENT '"+ f.FieldComment + "',");
            return sb.ToString();
        }

        protected static string getReorganize(FieldSettings f)
        {
            StringBuilder sb = new StringBuilder("");
            if (f.DataDbSettings.SelectDataType.TypeValue == "datetime")
            {
                sb.AppendLine(@"        $data['" + f.FieldName+"'] = $this->changeToMongoDate($data['"+ f.FieldName + "']);");
            }

            if (f.DataDbSettings.SelectDataType.TypeValue == "boolean")
            {
                sb.AppendLine(@"        $data['" + f.FieldName + "'] = $this->changeToBoolean($data['" + f.FieldName + "']);");
            }            

            if (f.DataDbSettings.SelectDataType.TypeValue == "json" || f.DataDbSettings.SelectDataType.TypeValue == "array")
            {
                sb.AppendLine(@"        $data['" + f.FieldName + "'] = $this->changeToArray($data['" + f.FieldName + "']);");
            }
            return sb.ToString();
        }
    }
}
