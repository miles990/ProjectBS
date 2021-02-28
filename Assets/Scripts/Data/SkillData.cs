using KahaGameCore.Interface;
using KahaGameCore.Static;

namespace ProjectBS.Data
{
    public class SkillData : IGameData
    {
        public int ID { get; private set; }
        public int SP { get; private set; }
        public int NameContextID { get; private set; }
        public int DescriptionContextID { get; private set; }
        public int StoryContextID { get; private set; }
        public string AnimationInfo { get; private set; }
        public string ReferenceBuffIDs { get; private set; }
        public string Command { get; private set; }
        public int IsDrop { get; private set; }
        public string Tag { get; private set; }

        public string GetAllDescriptionContext()
        {
            string _description = "";
            if (!string.IsNullOrEmpty(Tag))
            {
                string[] _tags = Tag.Split(';');
                for (int i = 0; i < _tags.Length; i++)
                {
                    _description += "[" + ContextConverter.Instance.GetContext(int.Parse(_tags[i])) + "]";
                }
            }

            if (!string.IsNullOrEmpty(_description))
            {
                _description += "\n";
            }

            _description += ContextConverter.Instance.GetContext(DescriptionContextID);

            if (!string.IsNullOrEmpty(ReferenceBuffIDs))
            {
                string[] _refBuffIDs = ReferenceBuffIDs.Split(';');
                for (int i = 0; i < _refBuffIDs.Length; i++)
                {
                    BuffData _refBuff = GameDataManager.GetGameData<BuffData>(_refBuffIDs[i].ToInt());

                    _description += "\n\n";
                    _description += ContextConverter.Instance.GetContext(_refBuff.NameContextID);
                    _description += "\n";
                    _description += _refBuff.Tag == 0 ? "" : "[" + ContextConverter.Instance.GetContext(_refBuff.Tag) + "]\n";
                    _description += ContextConverter.Instance.GetContext(_refBuff.DescriptionContextID);
                }
            }

            return _description;
        }
    }
}