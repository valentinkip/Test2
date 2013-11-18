using JetBrains.Annotations;

namespace ManchkinQuest.Core.Impl.Cards.DxM
{
  [UsedImplicitly]
  class ThiefCard : DxMCardBase, IClassCard
  {
    public ThiefCard() : base(Images.LoadImage("Cards/DxM/Thief.jpg"))
    {
    }

    public Class Class
    {
      get { return Class.Thief; }
    }
  }
}