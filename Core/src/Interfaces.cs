using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;
using ManchkinQuest.Common;

namespace ManchkinQuest.Core
{
  public interface ICard
  {
    BitmapSource FaceImage { get; }
    BitmapSource BackImage { get; }
  }

  public interface ITreasureCard : ICard
  {
    int Price { get; }
  }

  public interface IMonsterCard : ICard
  {    
  }

  public interface IDxMCard : ICard
  {    
  }

  public interface IRaceCard : IDxMCard
  {
    Race Race { get; }
  }

  public interface IClassCard : IDxMCard
  {
    Class Class { get; }
  }

  public interface IHalfBreededCard : IDxMCard
  {    
  }

  public interface ISuperManchkinCard : IDxMCard
  {    
  }

  public interface ICheatCard : IDxMCard
  {    
  }

  public interface ICardDeck<TCard> where TCard : class, ICard
  {
    IList<TCard> Cards { get; } // card to take is the first card in the list
    IList<TCard> Discards { get; } // the last discarded card is the last one in the list
    TCard TakeCard();
    void DiscardCard(TCard card);
  }

  public interface IPlayer
  {
    string Name { get; }
    PlayerColor Color { get; }
    BitmapSource Image { get; }

    int Level { get; }
    Sex Sex { get; }

    IList<Tuple<ITreasureCard, ICheatCard>> WornItems { get; }
    IList<ITreasureCard> InPackItems { get; }

    IList<IRaceCard> RaceCards { get; }
    [CanBeNull] IHalfBreededCard HalfBreededCard { get; }
    Race Race { get; }
    bool HasRaceAdvantagesOnly { get; }

    IList<IClassCard> ClassCards { get; }
    Class Class { get; }
    [CanBeNull] ISuperManchkinCard SuperManchkinCard { get; }
    bool HasClassAdvantagesOnly { get; }

    IList<ICard> HandCards { get; }

    int Golds { get; }

    int HealthTokensTotal { get; }
    int HealthTokensActive { get; }

    int StepsPerformed { get; }

    IMazeRoomInfo Location { get; set; }
  }

  [Flags]
  public enum Race
  {
    Human = 0,
    Elf = 1,
    Dwarf = 2,
    Halfling = 4
  }

  [Flags]
  public enum Class
  {
    None = 0,
    Warrior = 1,
    Wizard = 2,
    Thief = 4,
    Cleric = 8
  }

  public interface IRoom
  {
    BitmapSource Image { get; }
  }

  public interface IEnterRoom : IRoom
  {    
  }

  public enum Direction { Left, Right, Up, Down }

  public interface INormalRoom : IRoom
  {
    Color ArrowColor(Direction direction);
  }

  public interface IRoomCard
  {
    INormalRoom Side1 { get; }
    INormalRoom Side2 { get; }
  }

  public static class RoomArrowColors
  {
    public static readonly Color Red = Colors.Red;
    public static readonly Color Green = Colors.Green;
    public static readonly Color Blue = Colors.Blue;
    public static readonly Color Yellow = Colors.Yellow;
    public static readonly Color Cyan = Colors.Cyan;
    public static readonly Color Orange = Colors.Orange;
  }

  public interface IRoomLink
  {
    BitmapSource Image { get; }
    int Steps { get; }
  }

  public interface IMaze
  {
    IMazeRoomInfo Room(int x, int y);
    IMazeRoomInfo Enter { get; }
    IEnumerable<IMazeRoomInfo> AllRooms { get; }
    IMazeRoomInfo AddRoom(int x, int y, IRoom room);
  }

  public interface IMazeRoomInfo
  {
    int X { get; }
    int Y { get; }
    IRoom Room { get; }
    IRoomLink Link(Direction direction);
    void SetLink(Direction direction, IRoomLink link);
  }

  public interface IGame
  {
    IList<IPlayer> Players { get; }
    IPlayer ActivePlayer { get; }
    IPlayer PlayerByName(string name);
    IMaze Maze { get; }
    ICardDeck<IDxMCard> DxMCardDeck { get; }
    ICardDeck<ITreasureCard> TreasureCardDeck { get; }
    ICardDeck<IMonsterCard> MonsterCardDeck { get; }
    void InitializeDecks(int seed);
  }

  public interface IGameEvent
  {
    void Apply(IGame game);
    void WriteTo(BinaryWriter writer);
    void ReadFrom(BinaryReader reader);
  }
}