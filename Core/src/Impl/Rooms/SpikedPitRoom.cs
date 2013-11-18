namespace ManchkinQuest.Core.Impl.Rooms
{
  class SpikedPitRoom : NormalRoomBase
  {
    public SpikedPitRoom()
      : base(Images.LoadImage("Rooms/SpikedPit.png"),
             left: RoomArrowColors.Red, right: RoomArrowColors.Orange, up: RoomArrowColors.Blue, down: RoomArrowColors.Cyan)
    {
    }
  }
}