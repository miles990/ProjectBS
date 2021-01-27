using UnityEngine;

namespace ProjectBS.UI
{
    public static class UITextColorStorer
    {
        public static readonly Color RankSTextColor = new Color(255f / 255f, 76f / 255f, 76f / 255f);
        public static readonly Color RankATextColor = new Color(255f / 255f, 76f / 255f, 240f / 255f);
        public static readonly Color RankBTextColor = new Color(76f / 255f, 162f / 255f, 255f / 255f);
        public static readonly Color RankCTextColor = new Color(76f / 255f, 255f / 255f, 90f / 255f);
        public static readonly Color RankDTextColor = new Color(181f / 255f, 192f / 255f, 182f / 255f);
        public static readonly Color RankETextColor = new Color(236f / 255f, 236f / 255f, 236f / 255f);
        public static readonly Color RankFTextColor = new Color(255f / 255f, 255f / 255f, 255f / 255f);
        public static readonly Color RankTTextColor = new Color(87f / 255f, 87f / 255f, 87f / 255f);

        public static Color GetRankStringColor(string rank)
        {
            switch (rank)
            {
                case "S": return RankSTextColor;
                case "A": return RankATextColor;
                case "B": return RankBTextColor;
                case "C": return RankCTextColor;
                case "D": return RankDTextColor;
                case "E": return RankETextColor;
                case "F": return RankFTextColor;
                case "T": return RankTTextColor;
            }
            return new Color();
        }
    }
}
