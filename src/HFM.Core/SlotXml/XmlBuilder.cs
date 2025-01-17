﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

using AutoMapper;

using HFM.Core.Client;
using HFM.Core.Serializers;
using HFM.Preferences;

namespace HFM.Core.SlotXml
{
    public readonly struct XmlBuilderResult
    {
        public XmlBuilderResult(string slotSummaryFile, ICollection<string> slotDetailFiles)
        {
            SlotSummaryFile = slotSummaryFile;
            SlotDetailFiles = slotDetailFiles;
        }

        public string SlotSummaryFile { get; }

        public ICollection<string> SlotDetailFiles { get; }
    }

    public class XmlBuilder
    {
        public IPreferences Preferences { get; }

        private readonly IMapper _mapper;

        public XmlBuilder(IPreferences preferences)
        {
            Preferences = preferences;
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<XmlBuilderProfile>()).CreateMapper();
        }

        public XmlBuilderResult Build(ICollection<SlotModel> slots, string path)
        {
            if (slots == null) throw new ArgumentNullException(nameof(slots));

            var updateDateTime = DateTime.Now;
            var slotSummaryFile = CreateSlotSummaryFile(slots, path, updateDateTime);
            var slotDetailFiles = EnumerateSlotDetailFiles(slots, path, updateDateTime).ToList();
            return new XmlBuilderResult(slotSummaryFile, slotDetailFiles);
        }

        private const string SlotSummaryXml = "SlotSummary.xml";

        private string CreateSlotSummaryFile(ICollection<SlotModel> slots, string path, DateTime updateDateTime)
        {
            var slotSummary = CreateSlotSummary(slots, updateDateTime);

            var serializer = new DataContractFileSerializer<SlotSummary>();
            string filePath = Path.Combine(path, SlotSummaryXml);
            serializer.Serialize(filePath, slotSummary);
            return filePath;
        }

        internal SlotSummary CreateSlotSummary(ICollection<SlotModel> slots, DateTime updateDateTime)
        {
            var slotSummary = new SlotSummary();
            slotSummary.HfmVersion = Application.FullVersion;
            slotSummary.NumberFormat = NumberFormat.Get(Preferences.Get<int>(Preference.DecimalPlaces), XsltNumberFormat);
            slotSummary.UpdateDateTime = updateDateTime;
            slotSummary.SlotTotals = SlotTotals.Create(slots);
            slotSummary.Slots = SortSlots(slots).Select(_mapper.Map<SlotModel, SlotData>).ToList();
            return slotSummary;
        }

        private IEnumerable<SlotModel> SortSlots(IEnumerable<SlotModel> slots)
        {
            string sortColumn = Preferences.Get<string>(Preference.FormSortColumn);
            if (String.IsNullOrWhiteSpace(sortColumn))
            {
                return slots;
            }

            var property = TypeDescriptor.GetProperties(typeof(SlotModel)).OfType<PropertyDescriptor>().FirstOrDefault(x => x.Name == sortColumn);
            if (property == null)
            {
                return slots;
            }

            var direction = Preferences.Get<ListSortDirection>(Preference.FormSortOrder);
            var sortComparer = new SlotModelSortComparer { OfflineClientsLast = Preferences.Get<bool>(Preference.OfflineLast) };
            sortComparer.SetSortProperties(property, direction);
            return slots.OrderBy(x => x, sortComparer);
        }

        private IEnumerable<string> EnumerateSlotDetailFiles(ICollection<SlotModel> slots, string path, DateTime updateDateTime)
        {
            var serializer = new DataContractFileSerializer<SlotDetail>();
            foreach (var slot in slots)
            {
                var slotDetail = CreateSlotDetail(slot, updateDateTime);
                string filePath = Path.Combine(path, String.Concat(slot.Name, ".xml"));
                serializer.Serialize(filePath, slotDetail);
                yield return filePath;
            }
        }

        internal SlotDetail CreateSlotDetail(SlotModel slot, DateTime updateDateTime)
        {
            var slotDetail = new SlotDetail();
            slotDetail.HfmVersion = Application.FullVersion;
            slotDetail.NumberFormat = NumberFormat.Get(Preferences.Get<int>(Preference.DecimalPlaces), XsltNumberFormat);
            slotDetail.UpdateDateTime = updateDateTime;
            slotDetail.LogFileAvailable = Preferences.Get<bool>(Preference.WebGenCopyFAHlog);
            slotDetail.LogFileName = slot.Settings.ClientLogFileName;
            slotDetail.TotalRunCompletedUnits = slot.TotalRunCompletedUnits;
            slotDetail.TotalCompletedUnits = slot.TotalCompletedUnits;
            slotDetail.TotalRunFailedUnits = slot.TotalRunFailedUnits;
            slotDetail.TotalFailedUnits = slot.TotalFailedUnits;
            slotDetail.SlotData = _mapper.Map<SlotModel, SlotData>(slot);
            return slotDetail;
        }

        private const string XsltNumberFormat = "###,###,##0";
    }
}
