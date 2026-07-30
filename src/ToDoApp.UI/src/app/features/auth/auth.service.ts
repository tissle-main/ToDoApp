import { computed, inject, Service } from "@angular/core";
import { Api, LoginUserCommand, RegisterUserCommand } from "../../api";
import { AuthTokenStore } from "./auth.token.store";
import { Observable, tap } from "rxjs";

@Service()
export class AuthService
{
  private readonly api = inject(Api);
  private readonly authTokenStore = inject(AuthTokenStore);
  public readonly isAuthenticated = computed(this.authTokenStore.isAuthenticated);

  public registerUser(request: RegisterUserCommand): Observable<void>
  {
    return this.api.registerUser(request);
  }
  public loginUser(request: LoginUserCommand): Observable<string>
  {
    return this.api.loginUser(request).pipe(
      tap(this.authTokenStore.setToken)
    );
  }
  public deleteUser(): Observable<void>
  {
    return this.api.deleteUser().pipe(
      tap(this.authTokenStore.removeToken)
    );
  }
}
