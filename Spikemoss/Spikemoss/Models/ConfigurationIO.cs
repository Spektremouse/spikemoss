using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Spikemoss.Models
{
    class ConfigurationIO
    {
        private const string HOSTNAME = "Hostname";
        private const string ADDRESS = "Address";
        private const string PLATFORM = "Platform";
        private const string SYS_USERNAME = "System User";
        private const string SYS_PASSWORD = "System Password";
        private static readonly string[] FILE_HEADERS = { HOSTNAME, ADDRESS, PLATFORM, SYS_USERNAME, SYS_PASSWORD }; 

        private string[] _headers = { };
        private List<Server> _servers;
        private List<User> _users;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationIO"/> class.
        /// </summary>
        public ConfigurationIO()
        {
            _servers = new List<Server>();
            _users = new List<User>();
        }

        /// <summary>
        /// The list of imported servers.
        /// </summary>
        public List<Server> ServerList
        {
            get { return _servers; }
        }

        /// <summary>
        /// Returns the list of imported users.
        /// </summary>
        public List<User> UserList
        {
            get { return _users; }
        }

        /// <summary>
        /// Finds <see cref="Server"/> and <see cref="User"/>
        /// objects in the comma delimited file and adds them to the <see cref="ConfigurationIO"/>
        /// <see cref="Server"/> and <see cref="User"/> lists.
        /// </summary>
        /// <param name="filename">The name of the file includings its path.</param>
        public void ImportConfiguration(string filename)
        {
            if (filename != null)
            {
                using (var reader = new CsvReader(new StreamReader(filename), true))
                {
                    var headers = reader.GetFieldHeaders();

                    if (ValidateHeaders(headers))
                    {
                        bool unique = true;

                        while (reader.ReadNextRecord())
                        {
                            unique = true;
                            var server = new Server();
                            server.Hostname = reader[HOSTNAME];
                            server.Address = reader[ADDRESS];

                            string tempos = reader[PLATFORM];

                            if (tempos.Contains("Windows"))
                            {
                                server.OperatingSystem = OperatingSystemType.Windows;
                            }
                            else if (tempos.Contains("Linux"))
                            {
                                server.OperatingSystem = OperatingSystemType.Linux;
                            }
                            else if (tempos.Contains("AIX"))
                            {
                                server.OperatingSystem = OperatingSystemType.AIX;
                            }
                            else if (tempos.Contains("HPUX"))
                            {
                                server.OperatingSystem = OperatingSystemType.HPUX;
                            }
                            else if (tempos.Contains("Solaris"))
                            {
                                server.OperatingSystem = OperatingSystemType.Solaris;
                            }

                            var osUser = new User();
                            osUser.Name = reader[SYS_USERNAME];
                            osUser.Password = reader[SYS_PASSWORD];

                            server.User = osUser;

                            //decreases performance
                            foreach (var x in _servers)
                            {
                                if (server.Equals(x))
                                {
                                    unique = false;
                                    break;
                                }
                            }

                            if (unique)
                            {
                                _servers.Add(server);
                            }
                        }
                    }
                    else
                    {
                        throw new ArgumentException("File headers do match an importable configuration file.");
                    }
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public void ExportConfiguration(string filename, List<Server> serverList)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                int i = 0;
                foreach (var header in FILE_HEADERS)
                {
                    writer.Write(header);
                    if (i < FILE_HEADERS.Length - 1)
                    {
                        writer.Write(",");
                    }
                }

                writer.WriteLine();

                foreach (var server in serverList)
                {
                    var builder = new StringBuilder();
                    builder.Append(server.Hostname).Append(",")
                        .Append(server.Address).Append(",")
                        .Append(server.OperatingSystem.ToString()).Append(",")
                        .Append(server.User.Name);

                    writer.WriteLine(builder.ToString());
                }
            }
        }

        private bool ValidateHeaders(string[] headerArgs)
        {
            if (headerArgs.Length >= FILE_HEADERS.Length)
            {
                int headerFound = 0;
                foreach (var header in headerArgs)
                {
                    switch (header)
                    {
                        case HOSTNAME:
                            headerFound++;
                            break;
                        case ADDRESS:
                            headerFound++;
                            break;
                        case PLATFORM:
                            headerFound++;
                            break;
                        case SYS_USERNAME:
                            headerFound++;
                            break;
                        case SYS_PASSWORD:
                            headerFound++;
                            break;
                        default:                            
                            break;
                    }
                }
                if (headerFound == 5)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
