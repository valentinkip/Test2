using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ManchkinQuest.Core.Impl;
using ManchkinQuest.Core.Impl.Rooms;

namespace ManchkinQuest.Core
{
  public class StuffManager
  {
    public static readonly double CardImageWidthToHeight = (double)Images.DxMCardBack.PixelWidth/Images.DxMCardBack.PixelHeight;
    public static readonly double ManchkinImageWidthToHeight = (double)Images.RedManchkin.PixelWidth/Images.RedManchkin.PixelHeight;

    public readonly double RoomLinkWidthToHeight;
    public readonly double RoomLinkWidthToRoomSize;
    public const double RoomLinkJunctionToHeight = 0.25;
    public readonly double EnterRoomSizeToNormalRoomSize;

    private readonly OpenPassageway myOpenPassageway = new OpenPassageway();
    private readonly RegularDoor myRegularDoor = new RegularDoor();
    private readonly LockedDoor myLockedDoor = new LockedDoor();
    private readonly HiddenDoor myHiddenDoor = new HiddenDoor();
    private readonly Wall myWall = new Wall();

    public static readonly StuffManager Instance = new StuffManager();

    private StuffManager()
    {
      DxMCards = GetTypesByInterface(typeof(IDxMCard)).Select(type => (IDxMCard)Activator.CreateInstance(type)).ToList();
      TreasureCards = GetTypesByInterface(typeof(ITreasureCard)).Select(type => (ITreasureCard)Activator.CreateInstance(type)).ToList();
      MonsterCards = GetTypesByInterface(typeof(IMonsterCard)).Select(type => (IMonsterCard)Activator.CreateInstance(type)).ToList();

      RoomLinks = new IRoomLink[]
                    {
                      myOpenPassageway, myRegularDoor, myLockedDoor, myHiddenDoor, myWall
                    };
      RoomCards = new IRoomCard[]
                    {
                      new RoomCard(new HallOfShameRoom(), new SeeminglyEmptyRoom()),
                      new RoomCard(new BugHouseRoom(), new SpikedPitRoom()),
                    };
      EnterRoom = new EnterRoom();

      RoomLinkWidthToHeight = (double)myOpenPassageway.Image.PixelWidth / myOpenPassageway.Image.PixelHeight;
      RoomLinkWidthToRoomSize = (double)myOpenPassageway.Image.PixelWidth / RoomCards[0].Side1.Image.PixelWidth;
      EnterRoomSizeToNormalRoomSize = (double)EnterRoom.Image.PixelWidth / RoomCards[0].Side1.Image.PixelWidth;
    }

    public IList<IDxMCard> DxMCards { get; private set; }
    public IList<ITreasureCard> TreasureCards { get; private set; }
    public IList<IMonsterCard> MonsterCards { get; private set; }
    public IList<IRoomCard> RoomCards { get; private set; }
    public IEnterRoom EnterRoom { get; private set; }
    public IList<IRoomLink> RoomLinks { get; private set; }

    public IRoomLink OpenPassageway
    {
      get { return myOpenPassageway; }
    }

    public IRoomLink RegularDoor
    {
      get { return myRegularDoor; }
    }

    public IRoomLink LockedDoor
    {
      get { return myLockedDoor; }
    }

    public IRoomLink HiddenDoor
    {
      get { return myHiddenDoor; }
    }

    public IRoomLink Wall
    {
      get { return myWall; }
    }

    private static IEnumerable<Type> GetTypesByInterface(Type interfaceType)
    {
      return Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && interfaceType.IsAssignableFrom(type));
    }
  }
}