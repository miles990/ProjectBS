using KahaGameCore.Interface;

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
        public string ReferenceContextIDs { get; private set; }
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

            if (!string.IsNullOrEmpty(ReferenceContextIDs))
            {
                string[] _additionDescriptionIDs = ReferenceContextIDs.Split(';');
                for (int i = 0; i < _additionDescriptionIDs.Length; i++)
                {
                    _description += "\n";
                    if(i % 2 == 0) _description += "\n";
                    _description += ContextConverter.Instance.GetContext(int.Parse(_additionDescriptionIDs[i]));
                }
            }

            return _description;
        }
    }
}