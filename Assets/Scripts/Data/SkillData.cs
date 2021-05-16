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
        public string References { get; private set; }
        public string Command { get; private set; }
        public int IsDrop { get; private set; }
        public string Tag { get; private set; }

        public string GetAllDescriptionContext()
        {
            string _description = "Cost SP: " + SP + "\n";

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

            if (!string.IsNullOrEmpty(References))
            {
                string[] _infos = References.Split(';');

                for (int i = 0; i < _infos.Length; i++)
                {
                    string[] _details = _infos[i].Split(':');

                    switch(_details[0])
                    {
                        case "S":
                            {
                                SkillData _refSkill = GameDataManager.GetGameData<SkillData>(_details[1].ToInt());

                                _description += "\n\n";
                                _description += ContextConverter.Instance.GetContext(_refSkill.NameContextID);
                                _description += "\n";
                                if (!string.IsNullOrEmpty(_refSkill.Tag))
                                {
                                    string[] _tags = Tag.Split(';');
                                    for (int _refSkillTagIndex = 0; _refSkillTagIndex < _tags.Length; _refSkillTagIndex++)
                                    {
                                        _description += "[" + ContextConverter.Instance.GetContext(int.Parse(_tags[_refSkillTagIndex])) + "]";
                                    }
                                }
                                _description += ContextConverter.Instance.GetContext(_refSkill.DescriptionContextID);
                                break;
                            }
                        case "B":
                            {
                                BuffData _refBuff = GameDataManager.GetGameData<BuffData>(_details[1].ToInt());

                                _description += "\n\n";
                                _description += ContextConverter.Instance.GetContext(_refBuff.NameContextID);
                                _description += "\n";
                                _description += _refBuff.Tag == 0 ? "" : "[" + ContextConverter.Instance.GetContext(_refBuff.Tag) + "]\n";
                                _description += ContextConverter.Instance.GetContext(_refBuff.DescriptionContextID);
                                break;
                            }
                        default:
                            throw new System.Exception("[SkillData][GetAllDescriptionContext] invaild References keyword:" + _details[0]);
                    }
                }
            }

            return _description;
        }
    }
}