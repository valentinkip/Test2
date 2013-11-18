namespace ManchkinQuest.Core.Impl.Rooms
{
  class HallOfShameRoom : NormalRoomBase
  {
    public HallOfShameRoom()
      : base(Images.LoadImage("Rooms/HallOfShame.png"),
             left: RoomArrowColors.Orange, right: RoomArrowColors.Red, up: RoomArrowColors.Cyan, down: RoomArrowColors.Yellow)
    {
    }
  }
}