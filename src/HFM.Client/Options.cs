﻿
using System.ComponentModel;

using Newtonsoft.Json.Linq;

namespace HFM.Client
{
   public class Options : Message
   {
      private Options()
      {
         
      }

      [MessageProperty("assignment-servers")]
      public string AssignmentServers { get; set; }

      [MessageProperty("capture-directory")]
      public string CaptureDirectory { get; set; }

      [MessageProperty("capture-sockets")]
      public bool CaptureSockets { get; set; }

      [MessageProperty("checkpoint")]
      public int Checkpoint { get; set; }

      [MessageProperty("child")]
      public bool Child { get; set; }

      [MessageProperty("client-subtype")]
      public string ClientSubtype { get; set; }

      [MessageProperty("client-type")]
      public string ClientType { get; set; }

      // could be IP Address type
      [MessageProperty("command-address")]
      public string CommandAddress { get; set; }

      // could be IP Address type
      [MessageProperty("command-allow")]
      public string CommandAllow { get; set; }

      // could be IP Address type
      [MessageProperty("command-allow-no-pass")]
      public string CommandAllowNoPass { get; set; }

      // could be IP Address type
      [MessageProperty("command-deny")]
      public string CommandDeny { get; set; }

      // could be IP Address type
      [MessageProperty("command-deny-no-pass")]
      public string CommandDenyNoPass { get; set; }

      [MessageProperty("command-port")]
      public int CommandPort { get; set; }

      [MessageProperty("config-rotate")]
      public bool ConfigRotate { get; set; }

      [MessageProperty("config-rotate-dir")]
      public string ConfigRotateDir { get; set; }

      [MessageProperty("config-rotate-max")]
      public int ConfigRotateMax { get; set; }

      [MessageProperty("core-dir")]
      public string CoreDir { get; set; }

      [MessageProperty("core-key")]
      public string CoreKey { get; set; }

      [MessageProperty("core-prep")]
      public string CorePrep { get; set; }

      [MessageProperty("core-priority")]
      public string CorePriority { get; set; }

      [MessageProperty("core-server")]
      public string CoreServer { get; set; }

      [MessageProperty("cpu-affinity")]
      public bool CpuAffinity { get; set; }

      // could be enum type
      [MessageProperty("cpu-species")]
      public string CpuSpecies { get; set; }

      // SHOULD be enum type
      [MessageProperty("cpu-type")]
      public string CpuType { get; set; }

      [MessageProperty("cpu-usage")]
      public int CpuUsage { get; set; }

      [MessageProperty("cpus")]
      public int Cpus { get; set; }

      [MessageProperty("cycle-rate")]
      public int CycleRate { get; set; }

      [MessageProperty("cycles")]
      public int Cycles { get; set; }

      [MessageProperty("daemon")]
      public bool Daemon { get; set; }

      [MessageProperty("data-directory")]
      public string DataDirectory { get; set; }

      [MessageProperty("debug-sockets")]
      public bool DebugSockets { get; set; }

      [MessageProperty("dump-after-deadline")]
      public bool DumpAfterDeadline { get; set; }

      [MessageProperty("eval")]
      public string Eval { get; set; }

      [MessageProperty("exception-locations")]
      public bool ExceptionLocations { get; set; }

      [MessageProperty("exec-directory")]
      public string ExecDirectory { get; set; }

      [MessageProperty("exit-when-done")]
      public bool ExitWhenDone { get; set; }

      [MessageProperty("extra-core-args")]
      public string ExtraCoreArgs { get; set; }

      [MessageProperty("force-ws")]
      public string ForceWs { get; set; }

      [MessageProperty("gpu")]
      public bool Gpu { get; set; }

      [MessageProperty("gpu-assignment-servers")]
      public string GpuAssignmentServers { get; set; }

      [MessageProperty("gpu-device-id")]
      public string GpuDeviceId { get; set; }

      [MessageProperty("gpu-id")]
      public int GpuId { get; set; }

      [MessageProperty("gpu-index")]
      public string GpuIndex { get; set; }

      [MessageProperty("gpu-vendor-id")]
      public string GpuVendorId { get; set; }

      [MessageProperty("log")]
      public string Log { get; set; }

      [MessageProperty("log-color")]
      public bool LogColor { get; set; }

      [MessageProperty("log-crlf")]
      public bool LogCrlf { get; set; }

      [MessageProperty("log-date")]
      public bool LogDate { get; set; }

      [MessageProperty("log-debug")]
      public bool LogDebug { get; set; }

      [MessageProperty("log-domain")]
      public bool LogDomain { get; set; }

      [MessageProperty("log-domain-levels")]
      public string LogDomainLevels { get; set; }

      [MessageProperty("log-header")]
      public bool LogHeader { get; set; }

      [MessageProperty("log-level")]
      public bool LogLevel { get; set; }

      [MessageProperty("log-no-info-header")]
      public bool LogNoInfoHeader { get; set; }

      [MessageProperty("log-redirect")]
      public bool LogRedirect { get; set; }

      [MessageProperty("log-rotate")]
      public bool LogRotate { get; set; }

      [MessageProperty("log-rotate-dir")]
      public string LogRotateDir { get; set; }

      [MessageProperty("log-rotate-max")]
      public int LogRotateMax { get; set; }

      [MessageProperty("log-short-level")]
      public bool LogShortLevel { get; set; }

      [MessageProperty("log-simple-domains")]
      public bool LogSimpleDomains { get; set; }

      [MessageProperty("log-thread-id")]
      public bool LogThreadId { get; set; }

      [MessageProperty("log-time")]
      public bool LogTime { get; set; }

      [MessageProperty("log-to-screen")]
      public bool LogToScreen { get; set; }

      [MessageProperty("log-truncate")]
      public bool LogTruncate { get; set; }

      [MessageProperty("machine-id")]
      public int MachineId { get; set; }

      [MessageProperty("max-delay")]
      public int MaxDelay { get; set; }

      // could be enum type
      [MessageProperty("max-packet-size")]
      public string MaxPacketSize { get; set; }

      [MessageProperty("max-queue")]
      public int MaxQueue { get; set; }

      [MessageProperty("max-shutdown-wait")]
      public int MaxShutdownWait { get; set; }

      [MessageProperty("max-slot-errors")]
      public int MaxSlotErrors { get; set; }

      [MessageProperty("max-unit-errors")]
      public int MaxUnitErrors { get; set; }

      [MessageProperty("max-units")]
      public int MaxUnits { get; set; }

      [MessageProperty("memory")]
      public string Memory { get; set; }

      [MessageProperty("min-delay")]
      public int MinDelay { get; set; }

      [MessageProperty("next-unit-percentage")]
      public int NextUnitPercentage { get; set; }

      [MessageProperty("priority")]
      public string Priority { get; set; }

      [MessageProperty("no-assembly")]
      public bool NoAssembly { get; set; }
      
      // could be enum type
      [MessageProperty("os-species")]
      public string OsSpecies { get; set; }

      // SHOULD be enum type
      [MessageProperty("os-type")]
      public string OsType { get; set; }

      [MessageProperty("passkey")]
      public string Passkey { get; set; }

      [MessageProperty("password")]
      public string Password { get; set; }

      [MessageProperty("pause-on-battery")]
      public bool PauseOnBattery { get; set; }

      [MessageProperty("pause-on-start")]
      public bool PauseOnStart { get; set; }

      [MessageProperty("pid")]
      public bool Pid { get; set; }

      [MessageProperty("pid-file")]
      public string PidFile { get; set; }

      [MessageProperty("project-key")]
      public int ProjectKey { get; set; }

      [MessageProperty("respawn")]
      public bool Respawn { get; set; }

      [MessageProperty("script")]
      public string Script { get; set; }

      [MessageProperty("service")]
      public bool Service { get; set; }

      [MessageProperty("service-description")]
      public string ServiceDescription { get; set; }

      [MessageProperty("service-restart")]
      public bool ServiceRestart { get; set; }

      [MessageProperty("service-restart-delay")]
      public int ServiceRestartDelay { get; set; }

      [MessageProperty("smp")]
      public bool Smp { get; set; }

      [MessageProperty("stack-traces")]
      public bool StackTraces { get; set; }

      [MessageProperty("team")]
      public int Team { get; set; }

      [MessageProperty("threads")]
      public int Threads { get; set; }

      [MessageProperty("user")]
      public string User { get; set; }
      
      [MessageProperty("verbosity")]
      public int Verbosity { get; set; }

      public static Options Parse(Message message)
      {
         return Parse(message.Value, message);
      }

      public static Options Parse(string json, Message message) 
      {
         var options = new Options();
         foreach (var prop in JObject.Parse(json).Properties())
         {
            FahClient.SetObjectProperty(options, TypeDescriptor.GetProperties(options), prop);
         }
         options.SetMessageValues(message);
         return options;
      }
   }
}