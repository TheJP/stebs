using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Stebs5
{
    /// <summary>
    /// See documentation in <see cref="IConstants"/>.
    /// These could possibly be moved to a configuration file.
    /// </summary>
    public class Constants : IConstants
    {
        protected const string ResourcesPath = @"~\bin\Resources\";
        public const string InstructionsPath = ResourcesPath + "INSTRUCTION.data";
        public const string Rom1Path = ResourcesPath + "ROM1.data";
        public const string Rom2Path = ResourcesPath + "ROM2.data";
        public const string PluginsPath = @"~\Plugins\";
        protected string FromServerPath(string relativePath) => HttpContext.Current.Server.MapPath(relativePath);
        public string InstructionsAbsolutePath => FromServerPath(InstructionsPath);
        public string Rom1AbsolutePath => FromServerPath(Rom1Path);
        public string Rom2AbsolutePath => FromServerPath(Rom2Path);
        public string PluginsAbsolutePath => FromServerPath(PluginsPath);
        public TimeSpan MinimalRunDelay { get; } = TimeSpan.FromMilliseconds(10);
        public TimeSpan DefaultRunDelay { get; } = TimeSpan.FromMilliseconds(500);
    }
}
