using CommonUtility.Logging;
using System;
using System.IO;
using System.Reflection;

namespace Excalibur.Business
{
    /// <summary>
    /// 创建与删除断点文件
    /// </summary>
    class BreakpointOperator
    {
        static Logger mLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);

        static public bool Create(string filePath)
        {
            bool result = false;
            try
            {
                var fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)
                {
                    using (fileInfo.Create()) { }
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                mLogger.Warn($"Error while creating file:{filePath}, due to:{ex}");
            }

            return result;
        }

        static public bool Delete(string filePath)
        {
            bool result = false;
            try
            {
                if (Directory.Exists(filePath))
                {
                    Directory.Delete(filePath);
                }
                else if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                mLogger.Warn($"Error while deleting file or directory:{filePath}, due to:{ex}");
            }

            return result;
        }
    }
}
