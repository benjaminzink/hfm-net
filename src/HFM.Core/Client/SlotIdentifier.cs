﻿
using System;
using System.Collections.Generic;
using System.Globalization;

namespace HFM.Core.Client
{
    public readonly struct SlotIdentifier : IEquatable<SlotIdentifier>, IComparable<SlotIdentifier>, IComparable
    {
        internal const int NoSlotID = -1;

        public static SlotIdentifier AllSlots => new SlotIdentifier(0, "All Slots");
        
        private SlotIdentifier(int ordinal, string name)
        {
            Ordinal = ordinal;
            ClientIdentifier = new ClientIdentifier(name, null, ClientSettings.NoPort, Guid.Empty);
            SlotID = NoSlotID;
        }

        public SlotIdentifier(ClientIdentifier clientIdentifier, int slotID)
        {
            Ordinal = Int32.MaxValue;
            ClientIdentifier = clientIdentifier;
            SlotID = slotID;
        }

        public int Ordinal { get; }
        
        public ClientIdentifier ClientIdentifier { get; }

        public int SlotID { get; }

        public string Name => AppendSlotID(ClientIdentifier.Name, SlotID);

        private static string AppendSlotID(string name, int slotId)
        {
            return slotId >= 0 ? String.Format(CultureInfo.InvariantCulture, "{0} Slot {1:00}", name, slotId) : name;
        }

        public override string ToString()
        {
            if (String.IsNullOrWhiteSpace(ClientIdentifier.Server)) return Name;
            return String.Format(CultureInfo.InvariantCulture, "{0} ({1})", Name, ClientIdentifier.ToServerPortString());
        }

        public bool Equals(SlotIdentifier other)
        {
            return Ordinal == other.Ordinal && ClientIdentifier.Equals(other.ClientIdentifier) && SlotID == other.SlotID;
        }

        public override bool Equals(object obj)
        {
            return obj is SlotIdentifier other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Ordinal;
                hashCode = (hashCode * 397) ^ ClientIdentifier.GetHashCode();
                hashCode = (hashCode * 397) ^ SlotID;
                return hashCode;
            }
        }

        private sealed class ProteinBenchmarkSlotIdentifierEqualityComparer : IEqualityComparer<SlotIdentifier>
        {
            public bool Equals(SlotIdentifier x, SlotIdentifier y)
            {
                return x.Ordinal == y.Ordinal && ClientIdentifier.ProteinBenchmarkEqualityComparer.Equals(x.ClientIdentifier, y.ClientIdentifier) && x.SlotID == y.SlotID;
            }

            public int GetHashCode(SlotIdentifier obj)
            {
                unchecked
                {
                    var hashCode = obj.Ordinal;
                    hashCode = (hashCode * 397) ^ ClientIdentifier.ProteinBenchmarkEqualityComparer.GetHashCode(obj.ClientIdentifier);
                    hashCode = (hashCode * 397) ^ obj.SlotID;
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<SlotIdentifier> ProteinBenchmarkEqualityComparer { get; } = new ProteinBenchmarkSlotIdentifierEqualityComparer();

        public static bool operator ==(SlotIdentifier left, SlotIdentifier right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SlotIdentifier left, SlotIdentifier right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(SlotIdentifier other)
        {
            var ordinalComparison = Ordinal.CompareTo(other.Ordinal);
            if (ordinalComparison != 0) return ordinalComparison;
            var clientComparison = ClientIdentifier.CompareTo(other.ClientIdentifier);
            if (clientComparison != 0) return clientComparison;
            return SlotID.CompareTo(other.SlotID);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is SlotIdentifier other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(SlotIdentifier)}");
        }

        public static bool operator <(SlotIdentifier left, SlotIdentifier right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(SlotIdentifier left, SlotIdentifier right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(SlotIdentifier left, SlotIdentifier right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(SlotIdentifier left, SlotIdentifier right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
