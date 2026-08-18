import { computed, inject, Service, signal } from "@angular/core";
import { Api, LoginUserCommand, LoginUserResponse, RefreshAccessTokenResponse, RegisterUserCommand, UserDto } from "../../api";
import { AuthTokenStore } from "./auth.token.store";
import { Observable, tap } from "rxjs";

@Service()
export class AuthService
{
  private readonly api = inject(Api);
  private readonly authTokenStore = inject(AuthTokenStore);
  public readonly user = signal<UserDto | null>(null);
  public readonly isAuthenticated = computed(() => this.authTokenStore.isAuthenticated());

  public registerUser(request: RegisterUserCommand): Observable<void>
  {
    return this.api.registerUser(request);
  }
  public loginUser(request: LoginUserCommand): Observable<LoginUserResponse>
  {
    return this.api.loginUser(request).pipe(
      tap(response => this.authTokenStore.setToken(response.accessToken)),
      tap(response => this.user.set(response.user))
    );
  }
  public refreshAccessToken(): Observable<RefreshAccessTokenResponse>
  {
    return this.api.refreshAccessToken().pipe(
      tap(response => this.authTokenStore.setToken(response.accessToken)),
      tap(response => this.user.set(response.user))
    );
  }
  public logoutUser()
  {
    this.authTokenStore.removeToken();
  }
  public deleteUser(): Observable<void>
  {
    return this.api.deleteUser().pipe(
      tap(() => this.logoutUser())
    );
  }
}
