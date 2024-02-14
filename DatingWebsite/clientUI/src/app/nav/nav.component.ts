import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent  implements OnInit {
  currentUser$: Observable<User |null> = of(null);
  model: any = {}
  constructor( public accountService :AccountService, private router:Router, private toster:ToastrService) { }

  ngOnInit(): void {
    
  }

  logIn(){
    this.accountService.login(this.model).subscribe({
      next: () =>{
        this.toster.success('Scussesfull login');
        this.router.navigateByUrl('/members');
        this.model ={};
      }
    })

  }
  logout(){
    this.accountService.logout();
    this.toster.warning('you are logged out!🙂');
    this.router.navigateByUrl('/');

  }

}