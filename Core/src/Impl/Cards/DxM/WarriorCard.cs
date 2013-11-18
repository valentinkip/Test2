using JetBrains.Annotations;

namespace ManchkinQuest.Core.Impl.Cards.DxM
{
  [UsedImplicitly]
  class WarriorCard : DxMCardBase, IClassCard
  {
    public WarriorCard() : base(Images.LoadImage("Cards/DxM/Warrior.jpg"))
    {
    }

    public Class Class
    {
      get { return Class.Warrior; }
    }
  }
}