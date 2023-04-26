namespace ChessVariantsAPI.GameOrganization;

/// <summary>
/// Superclass for all exceptions regarding game organization.
/// </summary>
public class OrganizerException : Exception
{
    public OrganizerException() : base("Unknown error. If the issue persists please notify developers at https://github.com/ChessVariants") { }
    public OrganizerException(string message) : base(message) { }
}

/// <summary>
/// Exception for when a game is not found amongst the active games.
/// </summary>
public class GameNotFoundException : OrganizerException
{
    public GameNotFoundException() : base("The game you were looking for does not exist") { }
    public GameNotFoundException(string message) : base(message) { }
}

/// <summary>
/// Exception for when an editor is not found amongst the active games.
/// </summary>
public class EditorNotFoundException : OrganizerException
{
    public EditorNotFoundException() : base("The editor you were looking for does not exist") { }
    public EditorNotFoundException(string message) : base(message) { }
}

/// <summary>
/// Exception for when a player is not found.
/// </summary>
public class PlayerNotFoundException : OrganizerException
{
    public PlayerNotFoundException(string message) : base(message) { }
}

/// <summary>
/// Exception for when a game is full of players.
/// </summary>
public class GameFullException : OrganizerException
{
    public GameFullException() : base("Game is already full") { }
    public GameFullException(string message) : base(message) { }
}

/// <summary>
/// Exception for when all players have left a game
/// </summary>
public class GameEmptyException : OrganizerException
{
    public GameEmptyException() : base() { }
}

/// <summary>
/// Exception for when a supplied chess color is not one of the valid ones (black/white)
/// </summary>
public class InvalidColorException : OrganizerException
{
    public InvalidColorException(string message) : base(message) { }
}

/// <summary>
/// Exception for when a player of a certain type already exists.
/// </summary>
public class PlayerAlreadyExistsException : OrganizerException
{
    public PlayerAlreadyExistsException(string message) : base(message) { }
}
