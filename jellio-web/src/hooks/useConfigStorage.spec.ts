import { renderHook, waitFor } from '@testing-library/react';
import { useForm } from 'react-hook-form';
import { afterEach, describe, expect, it, vi } from 'vitest';
import { useConfigStorage } from './useConfigStorage';
import type { ConfigFormType } from '@/components/configForm/formSchema';
import { getConfigFromServer } from '@/services/backendService';

vi.mock('@/services/backendService', () => ({
  getConfigFromServer: vi.fn(),
}));

const movies = {
  key: 'aabbccdd11223344eeff00112233aabb',
  name: 'Movies',
  type: 'movies',
};

const shows = {
  key: '11223344556677889900aabbccddeeff',
  name: 'Shows',
  type: 'tvshows',
};

const renderStoredForm = (
  storedConfig: unknown,
  { accessToken }: { accessToken?: string } = {},
) => {
  localStorage.setItem('jelliopp_config', JSON.stringify(storedConfig));
  return renderHook(() => {
    const form = useForm<ConfigFormType>({
      defaultValues: { serverName: 'srv', libraries: [], sortOrders: {} },
    });
    useConfigStorage(form, accessToken, [movies, shows]);
    return form;
  });
};

afterEach(() => localStorage.clear());

describe('useConfigStorage', () => {
  it('should load a config stored before catalog sort orders existed', async () => {
    const { result } = renderStoredForm({
      libraries: [movies],
      publicBaseUrl: 'https://jellyfin.example.com',
    });

    await waitFor(() => {
      expect(result.current.getValues()).toMatchObject({
        libraries: [movies],
        publicBaseUrl: 'https://jellyfin.example.com',
        sortOrders: {},
      });
    });
  });

  it('should restore the stored catalog sort orders', async () => {
    const { result } = renderStoredForm({
      libraries: [movies],
      sortOrders: { [movies.key]: 'ReleaseDate' },
    });

    await waitFor(() => {
      expect(result.current.getValues('sortOrders')).toEqual({
        [movies.key]: 'ReleaseDate',
      });
    });
  });

  it('should prefer the catalog sort orders saved on the server', async () => {
    vi.mocked(getConfigFromServer).mockResolvedValue({
      catalogSortOrders: [{ libraryId: movies.key, sortOrder: 'Name' }],
    });

    const { result } = renderStoredForm(
      { sortOrders: { [movies.key]: 'ReleaseDate' } },
      { accessToken: 'tk' },
    );

    await waitFor(() => {
      expect(result.current.getValues('sortOrders')).toEqual({
        [movies.key]: 'Name',
      });
    });
  });

  it('should restore the libraries selected on the server in a fresh browser', async () => {
    vi.mocked(getConfigFromServer).mockResolvedValue({
      selectedLibraries: [shows.key],
    });

    const { result } = renderStoredForm({}, { accessToken: 'tk' });

    await waitFor(() => {
      expect(result.current.getValues('libraries')).toEqual([shows]);
    });
  });
});
