using System;
using System.Collections.Generic;
using System.Text;
using Renci.SshNet;
using System.Text.RegularExpressions;

namespace Spikemoss.Models.Gatherers
{
    class LinuxGatherer : IHardwareGatherer
    {
        private const string SHELL = "echo $SHELL";
        private const string USER = "echo $USER";
        private const string HOSTNAME = "hostname -s";
        private const string GREP_HOSTS = "grep `hostname` /etc/hosts";
        private const string UNAME = "uname";
        private const string UNAME_R = "uname -r";
        private const string CPUINFO = "cat /proc/cpuinfo";
        private const string GREP_CPUINFO_PROC = "grep \"processor\" /proc/cpuinfo | uniq | wc -l";
        private const string GREP_CPUINFO_PHYSID = "grep \"physical id\" /proc/cpuinfo | uniq | wc -l";
        private const string GREP_CPUINFO_COREID = "grep \"core id\" /proc/cpuinfo | uniq | wc -l";
        private const string DMI_PROC = "/usr/sbin/dmidecode --type processor";
        private const string DMI_SYS = "/usr/sbin/dmidecode --type system | egrep -i 'system information|manufacturer|product'";
        private const string DMI_MANU = "/usr/sbin/dmidecode | grep -m 1  \"Manufacturer\"";

        public LinuxGatherer()
        {
        }

        /// <summary>
        /// Checks if the given <see cref="Server"/> is configured for hardware collection.
        /// </summary>
        /// <exception cref="System.Net.Sockets.SocketException">Occurs when the hostname cannot be reached or
        /// resolved and/or the SSH port is incorrect.</exception>
        /// <exception cref="Renci.SshNet.Common.SshAuthenticationException">Occurs when the username or password is 
        /// incorrect.</exception>
        /// <param name="serverArgs"></param>
        /// <returns></returns>
        public bool IsHardwareDataCollectable(Server serverArgs)
        {
            if (serverArgs == null)
            {
                serverArgs.Error = "Server cannot be null for hardware data collection.";
                return false;
            }
            else if (String.IsNullOrEmpty(serverArgs.Address))
            {
                serverArgs.Error = "Server address cannot be null or empty.";
                return false;
            }
            else if (serverArgs.Hardware == null)
            {
                serverArgs.Error = "Server hardware cannot be null for hardware data collection.";
                return false;
            }
            else if (serverArgs.User == null)
            {
                serverArgs.Error = "Server user cannot be null for hardware data collection.";
                return false;
            }
            else if (String.IsNullOrEmpty(serverArgs.User.Name))
            {
                serverArgs.Error = "Server username cannot be null or empty for hardware data collection.";
                return false;
            }
            else if (String.IsNullOrEmpty(serverArgs.User.Password))
            {
                serverArgs.Error = "Server user password cannot be null or empty for hardware data collection.";
                return false;
            }

            using (SshClient sshClient = new SshClient(serverArgs.Address, serverArgs.SSHPort, serverArgs.User.Name, serverArgs.User.Password))
            {
                try
                {
                    sshClient.Connect();

                    string result;

                    var cmd = sshClient.CreateCommand("/usr/sbin/dmidecode");

                    result = cmd.Execute();
                    if (String.IsNullOrEmpty(result))
                    {
                        serverArgs.Error = "Could not execute command /usr/sbin/dmidecode.";
                        return false;
                    }
                    else
                    {
                        serverArgs.Error = "";
                        return true;
                    }
                    
                    /*var reader = new StreamReader(cmd.ExtendedOutputStream);

                    result = reader.ReadToEnd();
                    if (result.Contains("Permission denied"))
                    {
                        result = RunCommandWithSudoAccount(sshClient, serverArgs, "sudo /usr/sbin/dmidecode");
                         
                        if (result.Contains("is not in the sudoers file") || result.Contains("is not allowed to execute"))
                        {
                            serverArgs.Error = String.Format("User:{0} has insufficient privileges to execute /usr/sbin/dmidecode", serverArgs.User.Name);
                            return false;
                        }
                    }*/
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    //serverArgs.Error = "Host cannot be reached or resolved and/or incorrect SSH port.";
                    serverArgs.Error = ex.Message;
                    return false;
                }
                catch (Renci.SshNet.Common.SshAuthenticationException ex)
                {
                    //serverArgs.Error = "Access denied. Invalid username or password.";
                    serverArgs.Error = ex.Message;
                    return false;
                }
            }
        }

        /// <summary>
        /// Collects the Lime hardware data from a <see cref="Server"/> and populates
        /// the <see cref="ServerHardware"/> properties.
        /// </summary>
        /// <param name="serverArgs"></param>
        public void GatherHardwareData(Server serverArgs)
        {
            using (SshClient sshClient = new SshClient(serverArgs.Address, serverArgs.SSHPort, serverArgs.User.Name, serverArgs.User.Password))
            {
                sshClient.Connect();

                string result;

                //Collect Manufacturer
                //not a valid check if dmidecode fails
                var cmd = sshClient.CreateCommand(DMI_MANU);
                if (serverArgs.User.Name.Equals("root"))
                {
                    result = cmd.Execute();
                    if (!String.IsNullOrEmpty(result))
                    {
                        result = result.Substring(0, result.Length - 1);
                        serverArgs.Hardware.Manufacturer = Regex.Split(result, ": ")[1];
                        serverArgs.Hardware.Manufacturer.Trim();
                    }
                }
                else
                {
                    // dmidecode fails above as it needs su priviledge
                    // so we run sudo dmidecode, here:
                    // ask user for pwd
                    result = RunCommandWithSudoAccount(sshClient, serverArgs, "sudo " + DMI_MANU);
                    if (String.IsNullOrEmpty(result))
                    {
                        serverArgs.Error = "User has insufficient privileges to run command: dmidecode \n";
                    }
                    else
                    {
                        result = result.Substring(0, result.Length - 1);
                        serverArgs.Hardware.Manufacturer = Regex.Split(result, ": ")[1];
                    }
                }

                //Collect Model from Product Name
                //not a valid check if dmidecode fails
                cmd = sshClient.CreateCommand(DMI_SYS);
                result = cmd.Execute();
                if (serverArgs.User.Name.Equals("root"))
                {
                    if (!String.IsNullOrEmpty(result))
                    {
                        result = result.Substring(0, result.Length - 1);
                        string[] systemInfo = Regex.Split(result, "\n");
                        serverArgs.Hardware.Model = Regex.Split(systemInfo[2], ": ")[1];
                    }
                }
                else
                {
                    // dmidecode fails above as it needs su priviledge
                    // so we run sudo dmidecode, here:
                    // ask user for pwd
                    result = RunCommandWithSudoAccount(sshClient, serverArgs, "sudo " + DMI_SYS);
                    string[] systemInfo = Regex.Split(result, "\n");
                    serverArgs.Hardware.Model = Regex.Split(systemInfo[2], ": ")[1];
                }
                if (String.IsNullOrEmpty(result))
                {
                    serverArgs.Error = "User has insufficient privileges to run command: dmidecode \n";
                }

                //Collect Machine name
                result = sshClient.CreateCommand(HOSTNAME).Execute();
                result = result.Substring(0, result.Length - 1);
                serverArgs.Hardware.MachineName = result;

                //Collect Processor Identifier and Speed
                cmd = sshClient.CreateCommand(CPUINFO);
                result = cmd.Execute();
                if (!String.IsNullOrEmpty(result))
                {
                    result = result.Substring(0, result.Length - 1);
                    string[] processorsInformationArr = Regex.Split(result, "\n");
                    //Whitespaces for split are important here
                    string cpu = Regex.Split(processorsInformationArr[4], ": ")[1];
                    serverArgs.Hardware.ProcessorIdentifier = Regex.Split(cpu, " @")[0];
                    serverArgs.Hardware.ProcessorSpeed = Regex.Split(cpu, "@ ")[1];
                }

                //Collect number of logical cores
                cmd = sshClient.CreateCommand(GREP_CPUINFO_PROC);
                serverArgs.Hardware.TotalLogicalCores = Int32.Parse(cmd.Execute());

                //Collect number of processors
                cmd = sshClient.CreateCommand(GREP_CPUINFO_PHYSID);
                result = cmd.Execute();
                if (Int32.Parse(result) == 0)
                {
                    serverArgs.Hardware.SocketsPopulatedPhysical = 1;
                }
                else
                {
                    serverArgs.Hardware.SocketsPopulatedPhysical = Int32.Parse(result);
                }

                //Collect number of cores per processor
                cmd = sshClient.CreateCommand(GREP_CPUINFO_COREID);
                result = cmd.Execute();
                if (Int32.Parse(result) == 0)
                {
                    serverArgs.Hardware.TotalPhysicalCores = 1;
                }
                else
                {
                    serverArgs.Hardware.TotalPhysicalCores = Int32.Parse(result);
                }
            }
        }

        //doesnt check if nopassword is set in sudoers
        private string RunCommandWithSudoAccount(SshClient sshClient, Server serverArgs, string commandToRun)
        {
            /*
                This method is run if user is non-root. user has to be in the sudoers list, for this to work.
                Returns the result of running command
            */
            IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
            termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);
            ShellStream shellStream = sshClient.CreateShellStream("xterm", 80, 24, 800, 600, 1024, termkvp);

            //Get logged in
            string rep = shellStream.Expect(new Regex(@"[$>]"));

            //send command               
            shellStream.WriteLine(commandToRun);
            shellStream.Expect(new Regex(@"([$#>:])"));
            shellStream.WriteLine(serverArgs.User.Password);
            rep = shellStream.Expect(new Regex(@"[$#>]"));
            string[] parts = rep.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int partsLength = parts.Length;
            if (partsLength > 2)
            {
                // parse out the elements we do not want
                // we don't want the first element and if the last element contains the username, then we don't want that too
                StringBuilder commandOutput = new StringBuilder();
                for (int i = 1; i < partsLength - 1; i++)
                {
                    commandOutput.Append(parts[i]).Append("\n");
                }
                if (false == parts[partsLength - 1].Contains(serverArgs.User.Name))
                {
                    commandOutput.Append(parts[partsLength - 1]);
                }
                return commandOutput.ToString();
            }
            return null;
        }
    }
}
