using Common.Core;

namespace CodingCode.Model
{
    public class DnxOptions
    {
        private string _dnx;
        private string _dnu;
        public string Dnx { get { return _dnx; } set { _dnx = Check.NotNullOrWhiteSpace(value, nameof(value)); } }
        public string Dnu { get { return _dnu; } set { _dnu = Check.NotNullOrWhiteSpace(value, nameof(value)); } }
    }
}