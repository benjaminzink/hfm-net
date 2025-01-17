﻿/*
 * HFM.NET - Markup Data Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using HFM.Core.Client;

namespace HFM.Core.SlotXml
{
   [DataContract(Namespace = "")]
   public class SlotSummary
   {
      [DataMember(Order = 1)]
      public string HfmVersion { get; set; }

      [DataMember(Order = 2)]
      public string NumberFormat { get; set; }

      [DataMember(Order = 3)]
      public DateTime UpdateDateTime { get; set; }

      [DataMember(Order = 4)]
      public SlotTotals SlotTotals { get; set; }

      [DataMember(Order = 5)]
      public List<SlotData> Slots { get; set; }
   }
}
