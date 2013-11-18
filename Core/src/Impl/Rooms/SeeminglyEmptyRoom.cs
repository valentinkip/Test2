namespace ManchkinQuest.Core.Impl.Rooms
{
  class SeeminglyEmptyRoom : NormalRoomBase
  {
    public SeeminglyEmptyRoom()
      : base(Images.LoadImage("Rooms/SeeminglyEmpty.png"),
             left: RoomArrowColors.Orange, right:RoomArrowColors.Blue, up:RoomArrowColors.Yellow, down:RoomArrowColors.Green)
    {
    }
  }
}