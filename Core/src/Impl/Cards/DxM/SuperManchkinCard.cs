using JetBrains.Annotations;

namespace ManchkinQuest.Core.Impl.Cards.DxM
{
  [UsedImplicitly]
  class SuperManchkinCard : DxMCardBase, ISuperManchkinCard
  {
    public SuperManchkinCard() : base(Images.LoadImage("Cards/DxM/SuperManchkin.jpg"))
    {
    }
  }
}