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
        public const string InstructionsPath = @"~\bin\Resources\INSTRUCTION.data";
        public string InstructionsAbsolutePath
        {
            get
            {
                var server = HttpContext.Current.Server;
                return server.MapPath(InstructionsPath);
            }
        }
    }
}
