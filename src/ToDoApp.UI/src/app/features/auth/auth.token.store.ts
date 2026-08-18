import { computed, Service, signal } from "@angular/core";

@Service()
export class AuthTokenStore
{
  public readonly token = signal<string | null>(null);
  public readonly isAuthenticated = computed(() => this.token() !== null);

  public setToken(token: string)
  {
    this.token.set(token);
    localStorage.setItem("access_token", token);
  }
  public removeToken()
  {
    this.token.set(null);
    localStorage.removeItem("access_token");
  }
  public restoreToken()
  {
    this.token.set(localStorage.getItem("access_token"));
  }
}
