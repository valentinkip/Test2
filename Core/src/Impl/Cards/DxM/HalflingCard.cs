using JetBrains.Annotations;

namespace ManchkinQuest.Core.Impl.Cards.DxM
{
  [UsedImplicitly]
  class HalflingCard : DxMCardBase, IRaceCard
  {
    public HalflingCard() : base(Images.LoadImage("Cards/DxM/Halfling.jpg"))
    {
    }

    public Race Race
    {
      get { return Race.Halfling; }
    }
  }
}