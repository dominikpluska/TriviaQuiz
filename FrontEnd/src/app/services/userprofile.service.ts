import { Injectable } from "@angular/core";

@Injectable({ providedIn: 'root' })
export class UserProfileService{
    private userName = '';
    private isAdmin : boolean = false;

    get getUserName(){
        return this.userName;
    }

    get getIsAdmin(){
        return this.isAdmin;
    }

    updateUserName(userName: any){
        this.userName = userName;
    }

    updateIsAdmin(isAdmin: number){
        if(isAdmin === 1){
            this.isAdmin = true;
        }
        else{
            this.isAdmin = false;
        }
    }
}