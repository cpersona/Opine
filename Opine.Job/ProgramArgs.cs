using System;
using System.Collections.Generic;
using CommandLine;

namespace Opine.Job
{
    public class ProgramArgs
    {
        [Option('s', "stream", Required = true,
            HelpText = "Stream to read, e.g. commands")]
        public string StreamName { get; set; }

        [Option('t', "type", Required = false,
            HelpText = "Aggregate type to read, e.g. User")]
        public string StreamType { get; set; } = null;

        [Option('o', "offset", Required = false,
            DefaultValue = 0L,
            HelpText = "Offset to begin reading from")]
        public long QueuePosition { get; set; } = 0L;

        [Option('b', "buffer", Required = false,
            DefaultValue = 100,
            HelpText = "Read buffer message count")]
        public int BufferSize { get; set; } = 100;

        [OptionArray('a', "assemblies", Required = true,
            HelpText = "Plugin assemblies, e.g. {Path}/{To}/{Assembly}.dll")]
        public string[] AssemblyNames { get; set; }
    }
}