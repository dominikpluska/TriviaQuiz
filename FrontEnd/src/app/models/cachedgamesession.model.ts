import { CachedQuestion } from "./cachedquestion.model"

export interface CachedGameSession{
    GameSessionId : string
    Questions : [CachedQuestion]
    Score : number
    AnsweredQuestions : number
    TotalQuestionCount : number
    SessionTime : string
}