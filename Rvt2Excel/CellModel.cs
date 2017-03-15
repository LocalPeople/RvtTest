using System;

namespace Rvt2Excel
{
    internal class CellModel : IComparable<CellModel>
    {
        public string Family { get; internal set; }
        public string FamilySymbol { get; internal set; }
        public string Id { get; internal set; }

        public int CompareTo(CellModel other)
        {
            int compare = Family.CompareTo(other.Family);
            if (compare == 0)
            {
                compare = FamilySymbol.CompareTo(other.FamilySymbol);
            }
            return compare;
        }
    }
}