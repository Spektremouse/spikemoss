using System;
using System.Management;

namespace Spikemoss.Models.Gatherers
{
    class WindowsGatherer : IHardwareGatherer
    {
        private const string REGISTRY_BRANCH_LOCATION = @"Hardware\\DESCRIPTION\\System\\CentralProcessor";
        private const UInt32 HKEY_LOCAL_MACHINE = 0x80000002;

        public WindowsGatherer()
        {
        }

        /// <summary>
        /// Checks if the given <see cref="Server"/> is configured for hardware collection.
        /// </summary>
        /// <param name="serverArgs"></param>
        /// <exception cref="UnauthorizedAccessException">Occurs when the username and/or password is 
        /// incorrect or if the user has insufficient privileges.</exception>
        /// <exception cref="System.Runtime.InteropServices.COMException">
        /// Can occur for a variety of reasons. See documentation.</exception>
        /// <exception cref="System.Management.ManagementException">
        /// Will occur if using credentials for local connections.</exception>
        /// /// <exception cref="System.Net.Sockets.SocketException">Occurs when the <see cref="Server"/>
        /// cannot be reached or resolved.</exception>
        /// <returns>Returns true if hardware data can be collected successfully.</returns>
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

            try
            {
                ManagementScope scope;
                ConnectionOptions conOptions;
                ObjectQuery query;
                ManagementObjectSearcher managementSearcher;
                string conStr;

                conOptions = new ConnectionOptions();
                if (!Environment.MachineName.ToLower().Contains(serverArgs.Hostname.ToLower()))
                {
                    conOptions.Username = serverArgs.User.Name;
                    conOptions.Password = serverArgs.User.Password;
                }
                conOptions.Impersonation = ImpersonationLevel.Impersonate;
                conOptions.Timeout = TimeSpan.FromSeconds(3);

                conStr = "\\\\" + serverArgs.Address + "\\root\\cimv2";

                scope = new ManagementScope(conStr, conOptions);

                query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                managementSearcher = new ManagementObjectSearcher(scope, query);
                using (ManagementObjectCollection results = managementSearcher.Get())
                {
                    serverArgs.Error = "";
                    return true;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                serverArgs.Error = ex.Message;
                return false;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                serverArgs.Error = ex.Message;
                return false;
            }
            catch (ManagementException ex)
            {
                serverArgs.Error = ex.Message;
                return false;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                serverArgs.Error = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                serverArgs.Error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Collects the hardware data from a <see cref="Server"/> and populates
        /// the <see cref="Hardware"/> properties.
        /// </summary>
        /// <param name="serverArgs"></param>
        public void GatherHardwareData(Server serverArgs)
        {
            if (serverArgs.Hardware == null)
            {
                throw new ArgumentNullException("Server.Hardware");
            }
            else if (serverArgs.User == null)
            {
                throw new ArgumentNullException("Server.User");
            }
            else if (String.IsNullOrEmpty(serverArgs.User.Name))
            {
                throw new ArgumentException("User name cannot be null or empty.");
            }
            else if (String.IsNullOrEmpty(serverArgs.User.Password))
            {
                throw new ArgumentException("User password cannot be null or empty.");
            }

            ManagementScope scope;
            ConnectionOptions conOptions;
            ObjectQuery query;
            ManagementObjectSearcher managementSearcher;
            string conStr;

            conOptions = new ConnectionOptions();
            if (!System.Environment.MachineName.ToLower().Contains(serverArgs.Hostname.ToLower()))
            {
                conOptions.Username = serverArgs.User.Name;
                conOptions.Password = serverArgs.User.Password;
            }
            conOptions.Impersonation = ImpersonationLevel.Impersonate;
            conOptions.Timeout = TimeSpan.FromSeconds(3);

            conStr = "\\\\" + serverArgs.Address + "\\root\\cimv2";

            scope = new ManagementScope(conStr, conOptions);

            query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            managementSearcher = new ManagementObjectSearcher(scope, query);

            foreach (ManagementObject m in managementSearcher.Get())
            {
                serverArgs.Hardware.OperatingSystemName = m["Caption"].ToString();
                serverArgs.Hardware.MachineName = m["CSName"].ToString();
            }

            query.QueryString = "SELECT * FROM  Win32_ComputerSystem";
            managementSearcher.Query = query;

            foreach (ManagementObject m in managementSearcher.Get())
            {
                serverArgs.Hardware.Manufacturer = m["Manufacturer"].ToString();
                serverArgs.Hardware.Model = m["Model"].ToString();
                serverArgs.Hardware.SocketsPopulatedPhysical = Int32.Parse(m["NumberOfProcessors"].ToString());
            }

            query.QueryString = "SELECT * FROM Win32_Processor";
            managementSearcher.Query = query;

            foreach (ManagementObject m in managementSearcher.Get())
            {
                serverArgs.Hardware.ProcessorIdentifier = m["Name"].ToString();
                //serverArgs.Hardware.ProcessorType = m["Description"].ToString();
                serverArgs.Hardware.ProcessorSpeed = m["MaxClockSpeed"].ToString();
                serverArgs.Hardware.TotalPhysicalCores = Int32.Parse(m["NumberOfCores"].ToString());
                serverArgs.Hardware.TotalLogicalCores = Int32.Parse(m["NumberOfLogicalProcessors"].ToString());
            }
        }
    }
}
