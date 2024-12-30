import { Injectable } from "@angular/core";

@Injectable({ providedIn: 'root' })
export class UserProfileService{
    private userName = '';

    get getUserName(){
        return this.userName;
    }

    updateUserName(userName: any){
        this.userName = userName;
    }
}