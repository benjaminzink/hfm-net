﻿
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using HFM.Core.Net;

namespace HFM.Core.Client
{
    public readonly struct ClientIdentifier : IEquatable<ClientIdentifier>, IComparable<ClientIdentifier>, IComparable
    {
        public ClientIdentifier(string name, string server, int port, Guid guid)
        {
            Name = name;
            Server = server;
            Port = port;
            Guid = guid;
        }

        public string Name { get; }

        public string Server { get; }

        public int Port { get; }

        public Guid Guid { get; }

        public bool HasGuid => Guid != Guid.Empty;

        public override string ToString()
        {
            if (String.IsNullOrWhiteSpace(Server)) return Name;
            return String.Format(CultureInfo.InvariantCulture, "{0} ({1})", Name, ToServerPortString());
        }

        public bool Equals(ClientIdentifier other)
        {
            if (HasGuid || other.HasGuid)
            {
                return Guid.Equals(other.Guid);
            }
            return Name == other.Name && Server == other.Server && Port == other.Port;
        }

        public override bool Equals(object obj)
        {
            return obj is ClientIdentifier other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                if (HasGuid)
                {
                    return Guid.GetHashCode();
                }
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Server != null ? Server.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Port;
                return hashCode;
            }
        }

        private sealed class ProteinBenchmarkClientIdentifierEqualityComparer : IEqualityComparer<ClientIdentifier>
        {
            public bool Equals(ClientIdentifier x, ClientIdentifier y)
            {
                return x.Name == y.Name && x.Server == y.Server && x.Port == y.Port;
            }

            public int GetHashCode(ClientIdentifier obj)
            {
                unchecked
                {
                    var hashCode = (obj.Name != null ? obj.Name.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Server != null ? obj.Server.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.Port;
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<ClientIdentifier> ProteinBenchmarkEqualityComparer { get; } = new ProteinBenchmarkClientIdentifierEqualityComparer();

        public static bool operator ==(ClientIdentifier left, ClientIdentifier right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ClientIdentifier left, ClientIdentifier right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(ClientIdentifier other)
        {
            if (HasGuid)
            {
                return other.HasGuid ? Guid.CompareTo(other.Guid) : -1;
            }
            if (other.HasGuid)
            {
                return 1;
            }

            var nameComparison = String.Compare(Name, other.Name, StringComparison.Ordinal);
            if (nameComparison != 0) return nameComparison;
            var serverComparison = String.Compare(Server, other.Server, StringComparison.Ordinal);
            if (serverComparison != 0) return serverComparison;
            return Port.CompareTo(other.Port);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is ClientIdentifier other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(ClientIdentifier)}");
        }

        public static bool operator <(ClientIdentifier left, ClientIdentifier right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(ClientIdentifier left, ClientIdentifier right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(ClientIdentifier left, ClientIdentifier right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(ClientIdentifier left, ClientIdentifier right)
        {
            return left.CompareTo(right) >= 0;
        }

        public string ToServerPortString()
        {
            return TcpPort.Validate(Port) 
                ? String.Format(CultureInfo.InvariantCulture, "{0}:{1}", Server, Port) 
                : Server;
        }

        internal static readonly Regex ServerPortRegex = new Regex(@"(?<Server>.+)[-:](?<Port>\d+)$", RegexOptions.ExplicitCapture);

        internal static ClientIdentifier FromPath(string name, string path, Guid guid)
        {
            var match = path is null ? null : ServerPortRegex.Match(path);
            return match != null && match.Success 
                ? new ClientIdentifier(name, match.Groups["Server"].Value, Convert.ToInt32(match.Groups["Port"].Value), guid) 
                : new ClientIdentifier(name, path, ClientSettings.NoPort, guid);
        }
    }
}
