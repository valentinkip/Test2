using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using ManchkinQuest.Common;

namespace ManchkinQuest.Core.Impl
{
  public class Player : IPlayer
  {
    public Player(string name, PlayerColor color, Sex sex)
    {
      Name = name;
      Color = color;
      Sex = sex;
      Level = 1;
      WornItems = new List<Tuple<ITreasureCard, ICheatCard>>();
      InPackItems = new List<ITreasureCard>();
      RaceCards = new List<IRaceCard>();
      ClassCards = new List<IClassCard>();
      HandCards = new List<ICard>();
      Golds = 300;
      HealthTokensTotal = HealthTokensActive = 4;
      StepsPerformed = 0;
      Location = null; // to be set
    }

    public string Name { get; private set; }
    public PlayerColor Color { get; private set; }

    public BitmapSource Image
    {
      get
      {
        switch(Color)
        {
          case PlayerColor.Red:
            return Images.RedManchkin;
          case PlayerColor.Green:
            return Images.GreenManchkin;
          case PlayerColor.Blue:
            return Images.BlueManchkin;
          case PlayerColor.Yellow:
            return Images.YellowManchkin;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    public int Level { get; private set; }
    public Sex Sex { get; private set; }
    public IList<Tuple<ITreasureCard, ICheatCard>> WornItems { get; private set; }
    public IList<ITreasureCard> InPackItems { get; private set; }
    public IList<IRaceCard> RaceCards { get; private set; }
    public IHalfBreededCard HalfBreededCard { get; set; }
    public IList<IClassCard> ClassCards { get; private set; }
    public ISuperManchkinCard SuperManchkinCard { get; set; }
    public IList<ICard> HandCards { get; private set; }
    public int Golds { get; private set; }
    public int HealthTokensTotal { get; private set; }
    public int HealthTokensActive { get; private set; }
    public int StepsPerformed { get; private set; }

    public IMazeRoomInfo Location { get; set; }

    public Race Race
    {
      get { return RaceCards.Aggregate((Race)0, (current, card) => current | card.Race); }
    }

    public bool HasRaceAdvantagesOnly
    {
      get { return HalfBreededCard != null && RaceCards.Count == 1; }
    }

    public Class Class
    {
      get { return ClassCards.Aggregate((Class)0, (current, card) => current | card.Class); }
    }

    public bool HasClassAdvantagesOnly
    {
      get { return SuperManchkinCard != null && ClassCards.Count == 1; }
    }
  }
}