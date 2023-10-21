import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
 // @Input() usersFromHomeComponent :any;
  @Output() cancelRegister = new EventEmitter();
  model : any = {};
  
  constructor( private accountService: AccountService,
    private toster:ToastrService) { }
  ngOnInit(): void {
    
  }

  register(){
    this.accountService.register(this.model).subscribe({
      next: () =>{
        this.toster.success('Congraculation!😊, Registation Scussefull')
        this.cancel();
      },
      error: error => this.toster.error(error.error)
    })
  }
  cancel(){
    this.cancelRegister.emit(false);
  }

}
