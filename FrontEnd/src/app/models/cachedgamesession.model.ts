import { CachedQuestion } from "./cachedquestion.model"

export interface CachedGameSession{
    
    gameSessionId : string
    questions : [CachedQuestion]
    score : number
    answeredQuestions : number
    totalQuestionCount : number
    sessionTime : string

}