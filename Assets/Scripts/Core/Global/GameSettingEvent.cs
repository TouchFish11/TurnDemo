namespace Core.Global
{
    public delegate void GameSettingEvent<in T>(T value);
}