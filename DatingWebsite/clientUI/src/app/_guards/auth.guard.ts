import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toster = inject(ToastrService);
  const router = inject(Router);


  
  return accountService.currentUser$.pipe(
    map(user =>{
      if(user) return true;
      else{
        toster.error('you shall no pass!, sorry ☹');
        router.navigateByUrl('/');
        return false;
      }
    })
  )
};
