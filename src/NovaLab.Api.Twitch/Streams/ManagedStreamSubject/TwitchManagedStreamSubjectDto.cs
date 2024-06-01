// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using NovaLab.Data.Data.Twitch.Streams;

namespace NovaLab.Api.Twitch.Streams.ManagedStreamSubject;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public record TwitchManagedStreamSubjectDto(
    string SelectionName,
    string ObsSubjectTitle,
    string TwitchSubjectTitle
) {
    public static TwitchManagedStreamSubjectDto FromDbObject(TwitchManagedStreamSubject subject) {
        return new TwitchManagedStreamSubjectDto(
            SelectionName : subject.SelectionName,
            ObsSubjectTitle : subject.ObsSubjectTitle,
            TwitchSubjectTitle : subject.TwitchSubjectTitle
        );
    }    
} 
