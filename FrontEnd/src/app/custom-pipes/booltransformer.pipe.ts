import { Pipe, PipeTransform } from "@angular/core";

@Pipe({  name: 'booltransformer',
         standalone: true
                                })
export class BoolTransformerPipe implements PipeTransform{
    transform(value: number) : string {
        if(value === 1){
            return "Yes"
        }
        else if(value === 0){
            return "No"
        }
        else{
            return "Not handled"
        }
    }
}