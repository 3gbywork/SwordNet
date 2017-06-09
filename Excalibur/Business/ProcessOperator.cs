using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Utility.Logging;

namespace Excalibur.Business
{
    /// <summary>
    /// 对Process封装
    /// </summary>
    class ProcessOperator : IDisposable
    {
        static Logger mLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);

        public delegate void DataReceivedEventHandler(String message);
        public DataReceivedEventHandler OutputDataReceived;
        public DataReceivedEventHandler ErrorDataReceived;

        Process mProcess = null;
        ProcessStartInfo mProcessStartInfo = null;

        public ProcessOperator(ProcessStartInfo processStartInfo)
        {
            mProcessStartInfo = processStartInfo;
        }

        public bool Start()
        {
            bool isRunning = false;

            if (mProcess == null)
            {
                mProcess = CreateProcess(mProcessStartInfo);
            }

            try
            {
                mProcess.Start();
                if (mProcess.StartInfo.RedirectStandardOutput)
                {
                    mProcess.BeginOutputReadLine();
                }
                if (mProcess.StartInfo.RedirectStandardError)
                {
                    mProcess.BeginErrorReadLine();
                }
                isRunning = true;
            }
            catch (Exception ex)
            {
                mLogger.Error($"Error while starting process:{mProcess?.ProcessName}, due to:{ex}");
                Dispose();
                isRunning = false;
            }

            return isRunning;
        }

        public bool Stop()
        {
            Dispose();
            return true;
        }

        static public ProcessStartInfo GetProcessStartInfo(string fileName, string args = "", bool isRunAsAdmin = false)
        {
            return new ProcessStartInfo()
            {
                Arguments = args,
                CreateNoWindow = true,
                FileName = fileName,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                RedirectStandardInput = false,
                //StandardOutputEncoding = Encoding.UTF8,
                // "runas" "Edit", "Open", "OpenAsReadOnly", "Print", and "Printto".
                Verb = isRunAsAdmin ? "runas" : "",
                WorkingDirectory = Path.GetDirectoryName(fileName) ?? string.Empty,
                WindowStyle = ProcessWindowStyle.Hidden,
            };
        }

        public Process CreateProcess(ProcessStartInfo info)
        {
            Process process = null;

            if (Validation(info))
            {
                process = new Process
                {
                    EnableRaisingEvents = false,
                    StartInfo = info,
                };

                process.OutputDataReceived += (sender, e) =>
                {
                    OutputDataReceived?.Invoke(e.Data);
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    ErrorDataReceived?.Invoke(e.Data);
                };
            }

            return process;
        }

        static private bool Validation(ProcessStartInfo info)
        {
            if (info != null && !string.IsNullOrEmpty(info.FileName) && File.Exists(info.FileName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Dispose()
        {
            try
            {
                if (mProcess != null)
                {
                    using (mProcess)
                    {
                        if (mProcess.StartInfo.RedirectStandardOutput)
                        {
                            mProcess.CancelOutputRead();
                        }
                        if (mProcess.StartInfo.RedirectStandardError)
                        {
                            mProcess.CancelErrorRead();
                        }
                        // 如果进程没有退出
                        if (!mProcess.HasExited)
                        {
                            bool hasExited = false;
                            // 如果发送关闭消息成功，则在指定时间内等待程序退出
                            if (mProcess.CloseMainWindow())
                            {
                                hasExited = mProcess.WaitForExit(500);
                            }
                            if (!hasExited)
                            {
                                mProcess.Kill();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Warn($"Error while disposing process {mProcess?.ProcessName}, due to:{ex}");
            }
            finally
            {
                mProcess = null;
            }
        }
    }


}
