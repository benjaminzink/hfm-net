
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace HFM.Core.Client
{
    /// <summary>
    /// Client Types
    /// </summary>
    public enum ClientType
    {
        FahClient,
        [Obsolete("Do not use Legacy.")]
        Legacy
    }

    [DataContract(Namespace = "")]
    public class ClientSettings
    {
        public ClientSettings()
        {
            Port = DefaultPort;
        }

        public ClientIdentifier ClientIdentifier => new ClientIdentifier(Name, Server, Port, Guid);
        
        /// <summary>
        /// Gets or sets the client type.
        /// </summary>
        [DataMember(Order = 1)]
        public ClientType ClientType { get; set; }

        /// <summary>
        /// Gets or sets the client name.
        /// </summary>
        [DataMember(Order = 2)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the client host name or IP address.
        /// </summary>
        [DataMember(Order = 3)]
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the client host port number.
        /// </summary>
        [DataMember(Order = 4)]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the client host password.
        /// </summary>
        [DataMember(Order = 5)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the client unique identifier.
        /// </summary>
        [DataMember(Order = 6)]
        public Guid Guid { get; set; }

        private const string FahClientLogFileName = "log.txt";

        public string ClientLogFileName => String.Format(CultureInfo.InvariantCulture, "{0}-{1}", Name, FahClientLogFileName);

        public const int NoPort = 0;
        /// <summary>
        /// The default Folding@Home client port.
        /// </summary>
        public const int DefaultPort = 36330;

        private const string NameFirstCharPattern = "[a-zA-Z0-9\\+=\\-_\\$&^\\[\\]]";
        private const string NameMiddleCharsPattern = "[a-zA-Z0-9\\+=\\-_\\$&^\\[\\] \\.]";
        private const string NameLastCharPattern = "[a-zA-Z0-9\\+=\\-_\\$&^\\[\\]]";

        /// <summary>
        /// Validates the client settings name.
        /// </summary>
        public static bool ValidateName(string name)
        {
            if (name == null) return false;

            string pattern = String.Format(CultureInfo.InvariantCulture,
                "^{0}{1}+{2}$", NameFirstCharPattern, NameMiddleCharsPattern, NameLastCharPattern);
            return Regex.IsMatch(name, pattern, RegexOptions.Singleline);
        }

        /// <summary>
        /// Removes invalid characters from the client name string.
        /// </summary>
        public static string CleanName(string name)
        {
            if (name == null) return null;

            var first = new Regex(NameFirstCharPattern, RegexOptions.Singleline);
            var middle = new Regex(NameMiddleCharsPattern, RegexOptions.Singleline);
            var last = new Regex(NameLastCharPattern, RegexOptions.Singleline);

            var sb = new StringBuilder(name.Length);
            for (int i = 0; i < name.Length; i++)
            {
                var c = name[i];
                if (i == 0)
                {
                    if (first.IsMatch(c.ToString()))
                    {
                        sb.Append(c);
                    }
                }
                else if (i == name.Length - 1)
                {
                    if (last.IsMatch(c.ToString()))
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    if (middle.IsMatch(c.ToString()))
                    {
                        sb.Append(c);
                    }
                }
            }

            return sb.ToString();
        }
    }
}
